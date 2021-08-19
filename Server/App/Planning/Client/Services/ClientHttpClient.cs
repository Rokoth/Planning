using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using System.IO;
using Planning.Contract.Model;
using System.Net;

namespace Planning.Client.ClientHttpClient
{

    public class ClientHttpClient : IClientHttpClient
    {
        private readonly ILogger<ClientHttpClient> _logger;
        private bool isConnected = false;
        private bool isDisposed = false;
        private bool isCheckRun = false;
        private readonly Dictionary<Type, string> _apis;
        private string _server;

        private string _token { get; set; }

        public event EventHandler OnConnect;

        public bool IsConnected => isConnected;

        public ClientHttpClient(IHttpClientSettings settings, IServiceProvider serviceProvider)
        {
            _apis = settings.Apis;
            _logger = serviceProvider.GetRequiredService<ILogger<ClientHttpClient>>();
            if (!string.IsNullOrEmpty(settings.Server))
            {
                _server = settings.Server;
                Task.Factory.StartNew(CheckConnect, TaskCreationOptions.LongRunning);
                isCheckRun = true;
            }
        }

        public void ConnectToServer(string server, Action<bool, bool, string> onResult)
        {
            CheckConnectOnce(server).ContinueWith(s =>
            {
                if (s.IsFaulted)
                {
                    var message = "";
                    if (s.Exception is AggregateException aex)
                    {

                        var stack = "";
                        foreach (var ex in aex.InnerExceptions)
                        {
                            message += ex.Message + "\r\n";
                            stack += ex.StackTrace + "\r\n";
                        }

                        _logger.LogError($"Error in ConnectToServer: {message}; StackTrace: {stack}");
                    }
                    else
                    {
                        message = s.Exception.Message;
                        _logger.LogError($"Error in ConnectToServer: {s.Exception.Message}; StackTrace: {s.Exception.StackTrace}");
                    }
                    onResult?.Invoke(false, true, $"Error in ConnectToServer: {message};");
                }
                else
                {
                    var result = s.Result;
                    onResult?.Invoke(result, false, null);
                    if (result)
                    {
                        _server = server;
                        isConnected = true;
                        if (!isCheckRun)
                        {
                            Task.Factory.StartNew(CheckConnect, TaskCreationOptions.LongRunning);
                            isCheckRun = true;
                        }
                    }
                }
            });
        }

        public async Task<bool> Auth(UserIdentity identity)
        {
            var result = await Execute(client =>
                client.PostAsync($"{GetApi<UserIdentity>()}", identity.SerializeRequest()), "Auth", s => s.ParseResponse<ClientIdentityResponse>());
            if (result == null) return false;
            _token = result.Token;
            return true;
        }

        public async Task<ListResult<T>> Get<T>(string param, Type apiType = null) where T : class
        {
            return await Execute(client =>
                {
                    var request = new HttpRequestMessage()
                    {
                        Headers = {
                            { HttpRequestHeader.Authorization.ToString(), $"Bearer {_token}" },
                            { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        },
                        RequestUri = new Uri($"{GetApi<T>(apiType)}/{param}"),
                        Method = HttpMethod.Get
                    };

                    return client.SendAsync(request);                    
                }, "Get", s => s.ParseResponseArray<T>());
        }

        public async Task<T> GetItem<T>(Guid id, Type apiType = null) where T : class
        {
            return await Execute(client =>
            {
                var request = new HttpRequestMessage()
                {
                    Headers = {
                            { HttpRequestHeader.Authorization.ToString(), $"Bearer {_token}" },
                            { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        },
                    RequestUri = new Uri($"{GetApi<T>(apiType)}/{id}"),
                    Method = HttpMethod.Get
                };

                return client.SendAsync(request);
            }, "Get", s => s.ParseResponse<T>());
        }

        private async Task<T> Execute<T>(
            Func<HttpClient, Task<HttpResponseMessage>> action,
            string method,
            Func<HttpResponseMessage, Task<T>> parseMethod)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var result = await action(client);
                    return await parseMethod(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in {method}: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return default;
                }
            }
        }

        private string GetApi<T>(Type apiType = null) where T : class
        {
            return $"{_server}/{_apis[apiType ?? typeof(T)]}";
        }

        private async Task CheckConnect()
        {
            while (!isDisposed)
            {
                var curConnect = isConnected;
                isConnected = await CheckConnectOnce(_server);
                if (isConnected && !curConnect)
                {
                    OnConnect?.Invoke(this, new EventArgs());
                }
                await Task.Delay(1000);
            }
        }

        private async Task<bool> CheckConnectOnce(string server)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var check = await client.GetAsync($"{server}/api/v1/common/ping");
                    var result = check != null && check.IsSuccessStatusCode;
                    _logger.LogInformation($"Ping result: server {server} {(result ? "connected" : "disconnected")}");
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in CheckConnect: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return false;
                }
            }
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }











    public static class HttpApiHelper
    {
        public static StringContent SerializeRequest<TReq>(this TReq entity)
        {
            var json = JsonConvert.SerializeObject(entity);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            return data;
        }

        public static async Task<TResp> ParseResponse<TResp>(this HttpResponseMessage result) where TResp : class
        {
            if (result != null && result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                return JObject.Parse(response).ToObject<TResp>();
            }
            return null;
        }

        public static async Task<ListResult<T>> ParseResponseArray<T>(this HttpResponseMessage result) where T : class
        {
            if (result != null && result.IsSuccessStatusCode)
            {
                var ret = new List<T>();
                var response = await result.Content.ReadAsStringAsync();
                foreach (var item in JArray.Parse(response))
                {
                    ret.Add(item.ToObject<T>());
                }
                int count = 0;
                if (result.Headers.TryGetValues("x-pages", out IEnumerable<string> pageHeaders))
                {
                    int.TryParse(pageHeaders.FirstOrDefault(), out count);
                }
                return new ListResult<T>(count, ret);
            }
            return new ListResult<T>(0, null);
        }
    }

    
    public class ListResult<T> where T : class
    {
        public ListResult(int count, IEnumerable<T> items)
        {
            Count = count;
            Items = items;
        }
        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }

    public interface IDataService
    {
        Task<Project> AddProject(ProjectCreator projectCreator);
        Task<bool> DeleteProject(Guid id);
        Task<Project> GetProject(Guid id);
        Task<ListResult<Contract.Model.Project>> GetProjects(string name, int? page, int? size, string sort);
        Task<ListResult<Schedule>> GetSchedules(string name, int? page, int? size, string sort);
    }

    public class DataService : IDataService
    {
        public Task<Project> AddProject(ProjectCreator projectCreator)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProject(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Project> GetProject(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ListResult<Contract.Model.Project>> GetProjects(string name, int? page, int? size, string sort)
        {
            throw new NotImplementedException();
        }

        public Task<ListResult<Schedule>> GetSchedules(string name, int? page, int? size, string sort)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDbService
    {
        void SaveSettings(string v, string serverAddress);
    }

    public class DbService : IDbService
    {
        public void SaveSettings(string v, string serverAddress)
        {
            throw new NotImplementedException();
        }
    }
}
