using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> AuthApi(Contracts.Model.UserIdentity login, CancellationToken token);
        Task<ClaimsIdentity> Auth(Contracts.Model.UserIdentity login, CancellationToken token);
    }
}
