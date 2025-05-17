using Contracts.Model.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IAddDataService<Tdto, TCreator> where Tdto : Entity
    {
        Task<Tdto> AddAsync(TCreator entity, CancellationToken token);
    }
}
