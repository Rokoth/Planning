using Contracts.Model.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IUpdateDataService<Tdto, TUpdater> where Tdto : Entity
    {
        Task<Tdto> UpdateAsync(TUpdater entity, CancellationToken token);
    }
}
