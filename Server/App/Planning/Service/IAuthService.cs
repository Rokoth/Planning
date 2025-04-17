using Contracts.Model.User;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> AuthApi(UserIdentity login, CancellationToken token);
        Task<ClaimsIdentity> Auth(UserIdentity login, CancellationToken token);
    }
}
