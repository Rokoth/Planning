using Contracts.Model.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IGetDataService<Tdto, TFilter>
        where Tdto : Entity
        where TFilter : Filter<Tdto>
    {
        Task<Tdto> GetAsync(Guid id, CancellationToken token);
        Task<PagedResult<Tdto>> GetAsync(TFilter filter, CancellationToken token);
    }
}
