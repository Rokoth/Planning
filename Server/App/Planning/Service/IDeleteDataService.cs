using Contracts.Model.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IDeleteDataService<Tdto> where Tdto : Entity
    {
        Task<Tdto> DeleteAsync(Guid id, CancellationToken token);
    }
}
