using Contracts.Model.Common;
using Contracts.Model.User;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IUserDataService
    {
        Task<User> AddAsync(UserCreator creator, CancellationToken token);
        Task<User> DeleteAsync(Guid id, CancellationToken token);
        Task<User> GetAsync(Guid id, CancellationToken token);
        Task<PagedResult<User>> GetAsync(UserFilter filter, CancellationToken token);
        Task<User> UpdateAsync(UserUpdater entity, CancellationToken token);
    }
}