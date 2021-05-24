using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Planning.Common
{
    public enum ClientMode
    {
        ThickPriority = 1,
        ThinPriority = 2,
        ThickOnly = 3,
        ThinOnly = 4
    }

    public class CommonOptions
    {
        public string ConnectionString { get; set; }
        public ClientMode ClientMode { get; set; }
    }

    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
