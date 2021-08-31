using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Planning.Common
{
    public enum ResponseEnum
    {
        OK = 0,
        Error = 1,
        NeedAuth = 2
    }

    public class Response<TResp> where TResp : class
    {
        public ResponseEnum ResponseCode { get; set; }
        public TResp ResponseBody { get; set; }
    }

    public interface IErrorNotifyService
    {
        Task Send(string message, MessageLevelEnum level = MessageLevelEnum.Error, string title = null);
    }

    public class ErrorNotifyMessage
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public MessageLevelEnum MessageLevel { get; set; }
    }
    public enum MessageLevelEnum
    {
        Issue = 0,
        Warning = 1,
        Error = 10
    }

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
        public ErrorNotifyOptions ErrorNotifyOptions { get; set; }
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

    public class ErrorNotifyOptions
    {
        public bool SendError { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FeedbackContact { get; set; }
        public string DefaultTitle { get; set; }
    }
}
