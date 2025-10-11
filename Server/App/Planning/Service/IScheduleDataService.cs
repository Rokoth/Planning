using Contracts.Model.Common;
using Contracts.Model.Schedule;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IScheduleDataService
    {
        Task<PagedResult<Schedule>> GetListAsync(ScheduleFilter filter, CancellationToken token);
    }

    
}