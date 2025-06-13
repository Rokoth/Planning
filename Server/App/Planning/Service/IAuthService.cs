using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> AuthApi(Contract.Model.UserIdentity login, CancellationToken token);
        Task<ClaimsIdentity> Auth(Contract.Model.UserIdentity login, CancellationToken token);
    }
}
