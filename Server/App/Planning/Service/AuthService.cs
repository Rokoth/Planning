using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{

    public enum MessageLevelEnum
    {
        Issue = 0,
        Warning = 1,
        Error = 10
    }

    public class ErrorNotifyClientIdentity
    { 
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class ErrorNotifyClientIdentityResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }        
    }

    public class MessageCreator
    {        
        public int Level { get; set; } 
        public string Title { get; set; }       
        public string Description { get; set; }        
        public string FeedbackContact { get; set; }        
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
    }

    public interface IAuthService
    {
        Task<ClaimsIdentity> AuthApi(Contract.Model.UserIdentity login, CancellationToken token);
        Task<ClaimsIdentity> Auth(Contract.Model.UserIdentity login, CancellationToken token);
    }

    public class AuthService : IAuthService
    {
        private const string CLIENT_ROLE_TYPE = "Client";
        private const string TOKEN_AUTH_TYPE = "Token";
        private const string USER_ROLE_TYPE = "User";
        private const string COOKIES_AUTH_TYPE = "Cookies";

        private readonly IServiceProvider _serviceProvider;
        public AuthService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Client Auth
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> AuthApi(Contract.Model.UserIdentity login, CancellationToken token)
        {
            return await AuthInternal<DB.Context.User, Contract.Model.UserIdentity>(login, CLIENT_ROLE_TYPE, TOKEN_AUTH_TYPE, token);
        }

        /// <summary>
        /// User Auth
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> Auth(Contract.Model.UserIdentity login, CancellationToken token)
        {
            return await AuthInternal<DB.Context.User, Contract.Model.UserIdentity>(login, USER_ROLE_TYPE, COOKIES_AUTH_TYPE, token);
        }

        private async Task<ClaimsIdentity> AuthInternal<T, I>(I login, string roleType, string authType, CancellationToken token)
            where T : DB.Context.Entity, DB.Context.IIdentity
            where I : Contract.Model.IIdentity
        {
            var repo = _serviceProvider.GetRequiredService<DB.Repository.IRepository<T>>();
            var password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(login.Password));
            var client = (await repo.GetAsync(new DB.Context.Filter<T>()
            {
                Page = 0,
                Size = 10,
                Selector = s => s.Login == login.Login && s.Password == password
            }, token)).Data.FirstOrDefault();
            if (client != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, client.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, roleType)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователя/клиента не найдено
            return null;
        }
    }
}
