using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Planning.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Planning.Service
{
    public class ErrorNotifyService: IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ErrorNotifyService> _logger;

        private bool isConnected = false;
        private bool isAuth = false;
        private bool isDisposed = false;
        private bool isCheckRun = false;
        private bool _sendMessage = false;

        private string _server;
        private string _login;
        private string _password;
        private string _feedback;
        private string _defaultTitle;

        private object _lockObject = new object();

        private string _token { get; set; }        

        public ErrorNotifyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<ErrorNotifyService>>();
            var settings = serviceProvider.GetRequiredService<IOptions<CommonOptions>>().Value.ErrorNotifyOptions;
            if (settings.SendError)
            {
                if (!string.IsNullOrEmpty(settings.Server))
                {
                    _sendMessage = true;
                    _server = settings.Server;
                    _login = settings.Login;
                    _password = settings.Password;
                    _feedback = settings.FeedbackContact;
                    _defaultTitle = settings.DefaultTitle;

                    Task.Factory.StartNew(CheckConnect, TaskCreationOptions.LongRunning);
                    isCheckRun = true;
                }
                else
                {
                    _logger.LogError($"ErrorNotifyService error: Options.Server not set");
                }
            }
        }

        private async Task<bool> Auth()
        {
            var result = await Execute(client =>
                client.PostAsync($"{_server}/api/v1/client/auth", new ErrorNotifyClientIdentity()
                { 
                    Login = _login,
                    Password = _password
                }.SerializeRequest()), "Post", s => s.ParseResponse<ErrorNotifyClientIdentityResponse>());
            if (result == null)
            {
                if (isConnected)
                {
                    _logger.LogError($"ErrorNotifyService: Error in Auth method: wrong login or password");
                    _sendMessage = false;
                }                
                return false;
            }
            _token = result.Token;
            isAuth = true;
            return true;
        }

        public async Task Send(string message, MessageLevelEnum level, string title = null)
        {
            if (_sendMessage)
            {
                if (isAuth || await Auth())
                {
                    var result = await Execute(client =>
                    {
                        var request = new HttpRequestMessage()
                        {
                            Headers = {
                            { HttpRequestHeader.Authorization.ToString(), $"Bearer {_token}" },
                            { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        },
                            RequestUri = new Uri($"{_server}/api/v1/message/send"),
                            Method = HttpMethod.Post,
                            Content = new MessageCreator() { 
                                Description = message,
                                FeedbackContact = _feedback,
                                Level = (int)level,
                                Title = title ?? _defaultTitle
                            }.SerializeRequest()
                        };

                        return client.SendAsync(request);
                    }, "Send", s => s.ParseResponse<MessageCreator>());
                                       
                    if (result == null)
                    {
                        _logger.LogError($"ErrorNotifyService: Error in Send method: cant send message error");
                    }
                }
            }
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
                    if (isConnected)
                    {
                        var result = await action(client);
                        return await parseMethod(result);
                    }
                    _logger.LogError($"Error in {method}: server not connected");
                    return default;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in {method}: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return default;
                }
            }
        }

        private async Task CheckConnect()
        {
            while (!isDisposed)
            {               
                isConnected = await CheckConnectOnce(_server);                
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
}
