using FirebirdSql.EntityFrameworkCore.Firebird.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Planning.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    

    public class IntegrationService : IIntegrationService
    {
        #region ConstStrings

        private const string _requiredFieldError = "Обязательное поле {0} не задано";

        #endregion

        #region PrivateFields
        /// <summary>
        /// Flag: task collector service is connected
        /// </summary>
        private bool _isConnected = false;
        /// <summary>
        /// Flag: connect to task collector service is authed
        /// </summary>
        private bool _isAuth = false;
        /// <summary>
        /// Flag: service disposed
        /// </summary>
        private bool _isDisposed = false;
        /// <summary>
        /// Flag: can send message
        /// </summary>
        private bool _sendMessage = false;

        /// <summary>
        /// Required data for connect and send messages:
        /// - Server uri
        /// </summary>        
        private string _server;
        /// <summary>
        /// - Login
        /// </summary>
        private string _login;
        /// <summary>
        /// - Password
        /// </summary>
        private string _password;
               
        /// <summary>
        /// - Token for connect
        /// </summary>
        private string _token;


        /// <summary>
        /// Object for sync calls
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Calls locked
        /// </summary>
        private bool _isLock = false;

        /// <summary>
        /// Service inited
        /// </summary>
        private bool _init = false;

        private IntegrationOptions _options;


        #endregion
               
        /// <summary>
        /// ctor
        /// </summary>
        public IntegrationService(IServiceProvider serviceProvider)
        {
            if (_options == null)
            {
                var options = serviceProvider.GetRequiredService<IOptions<CommonOptions>>();
                _options = options.Value.IntegrationOptions;
            }
        }

        /// <summary>
        /// Init error notify logger
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Init()
        {            
            if (_options != null)
            {
                CheckRequired(_options.Server, nameof(_options.Server));
                CheckRequired(_options.Login, nameof(_options.Login));
                CheckRequired(_options.Password, nameof(_options.Password));

                _sendMessage = true;
                _server = _options.Server;
                _login = _options.Login;
                _password = _options.Password;              
                _isConnected = await CheckConnectOnce(_server);

                await Task.Factory.StartNew(CheckConnect, TaskCreationOptions.LongRunning);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check required field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="name"></param>
        private void CheckRequired(string field, string name)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new Exception(string.Format(_requiredFieldError, name));
            }
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <returns></returns>
        private async Task<bool> Auth()
        {
            bool _isLocked = false;
            lock (_lockObject)
            {
                if (_isLock)
                {
                    _isLocked = true;
                }
                else
                {
                    _isLock = true;
                }
            }
            if (_isLocked)
            {
                for (int i = 0; i < 60; i++)
                {
                    if (!_isLock)
                    {
                        break;
                    }
                    await Task.Delay(1000);
                }
                if (!_isLock)
                {
                    if (_isAuth) return true;
                    if (_isConnected) return false;
                }
                else
                {
                    Console.WriteLine($"Integration: Error in Auth method: cant wait for auth with lock");
                    return false;
                }
            }

            var result = await Execute(client =>
                client.PostAsync($"{_server}/api/v1/auth", new IntegrationIdentity()
                {
                    Login = _login,
                    Password = _password
                }.SerializeRequest()), "Post", s => s.ParseResponseExt<IntegrationIdentityResponse>(), false);

            if (result.ResponseCode == ResponseEnum.Error)
            {
                if (_isConnected)
                {
                    Console.WriteLine($"Integration: Error in Auth method: wrong login or password");
                    _sendMessage = false;
                }
                return false;
            }
            _token = result.ResponseBody.Token;
            _isAuth = true;
            lock (_lockObject)
            {
                _isLock = false;
            }
            return true;
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<S> Send<T, S>(T message, string path, HttpMethod method) where S: class
        {
            if (!_init) _init = await Init();
            if (_init && _sendMessage)
            {
                var result = await Execute(client =>
                {
                    var request = new HttpRequestMessage()
                    {
                        Headers = {
                            { HttpRequestHeader.Authorization.ToString(), $"Bearer {_token}" },
                            { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        },
                        RequestUri = new Uri($"{_server}/api/v1/{path}"),
                        Method = method,
                        Content = message.SerializeRequest()
                    };

                    return client.SendAsync(request);
                }, "Send", s => s.ParseResponseExt<S>());

                if (result.ResponseCode == ResponseEnum.Error)
                {
                    Console.WriteLine($"Integration: Error in Send method: cant send message error");
                    return null;
                }

                return result.ResponseBody;
            }
            return null;
        }

        /// <summary>
        /// обертка для вызова http
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="method"></param>
        /// <param name="parseMethod"></param>
        /// <param name="needAuth"></param>
        /// <returns></returns>
        private async Task<Response<T>> Execute<T>(
            Func<HttpClient, Task<HttpResponseMessage>> action,
            string method,
            Func<HttpResponseMessage, Task<Response<T>>> parseMethod, bool needAuth = true) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    if (_isConnected)
                    {
                        var result = await action(client);
                        var resp = await parseMethod(result);
                        if (resp.ResponseCode == ResponseEnum.NeedAuth)
                        {
                            if (needAuth && await Auth())
                            {
                                result = await action(client);
                                resp = await parseMethod(result);
                            }
                            else
                            {
                                return new Response<T>()
                                {
                                    ResponseCode = ResponseEnum.Error
                                };
                            }
                        }
                        return resp;
                    }
                    Console.WriteLine($"Error in {method}: server not connected");
                    return new Response<T>()
                    {
                        ResponseCode = ResponseEnum.Error
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in {method}: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return new Response<T>()
                    {
                        ResponseCode = ResponseEnum.Error
                    };
                }
            }
        }

        /// <summary>
        /// джоб проверки подключения к серверу
        /// </summary>
        /// <returns></returns>
        private async Task CheckConnect()
        {
            while (!_isDisposed)
            {
                _isConnected = await CheckConnectOnce(_server);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// проверка подключения к серверу
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        private async Task<bool> CheckConnectOnce(string server)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var check = await client.GetAsync($"{server}/api/v1/common/ping");
                    var result = check != null && check.IsSuccessStatusCode;
                    //Console.WriteLine($"Ping result: server {server} {(result ? "connected" : "disconnected")}");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in CheckConnect: {ex.Message}; StackTrace: {ex.StackTrace}");
                    return false;
                }
            }
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
        }

        public Task<bool> BuhgalteryAddReserve(string taskData, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<List<IntegrationProduct>> BuhgalteryGetProducts(string name, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    public class IntegrationProduct
    {

    }
}
