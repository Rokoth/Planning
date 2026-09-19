using Microsoft.Extensions.DependencyInjection;
using Planning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{

    public class AuthService : IAuthService
    {
        private const string CLIENT_ROLE_TYPE = "Client";
        private const string TOKEN_AUTH_TYPE = "Token";
        private const string USER_ROLE_TYPE = "User";
        private const string COOKIES_AUTH_TYPE = "Cookies";

        private readonly IServiceProvider _serviceProvider;       
        private readonly DB.Repository.IRepository<DB.Context.User> _repo;

        public AuthService(DB.Repository.IRepository<DB.Context.User> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Client Auth
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> AuthApi(Contracts.Model.UserIdentity login, CancellationToken token)
        {
            return await AuthInternal(login, CLIENT_ROLE_TYPE, TOKEN_AUTH_TYPE, token);
        }

        /// <summary>
        /// User Auth
        /// </summary>
        /// <param name="login"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> Auth(Contracts.Model.UserIdentity login, CancellationToken token)
        {
            return await AuthInternal(login, USER_ROLE_TYPE, COOKIES_AUTH_TYPE, token);
        }

        private async Task<ClaimsIdentity> AuthInternal(Contracts.Model.UserIdentity login, string roleType, string authType, CancellationToken token)           
        {            
            var password = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(login.Password));
            var client = (await _repo.GetAsync(new DB.Context.Filter<DB.Context.User>()
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
