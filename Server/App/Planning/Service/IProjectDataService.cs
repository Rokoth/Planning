using Contracts.Model.Common;
using Contracts.Model.Project;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectDataService
    {
        Task<PagedResult<Project>> GetListAsync(ProjectFilter filter, CancellationToken token);
    }

    
}