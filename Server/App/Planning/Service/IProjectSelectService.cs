using Contracts.Model.Schedule;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectSelectService
    {
        Task<Schedule> MoveToNextSchedule(Guid userId, 
            Guid? projectId = null, 
            Guid? directionIdId = null, 
            DateTimeOffset? beginDate = null);

        Task<IEnumerable<Schedule>> GetNextShedules(
            Guid userId,
            int count,
            DateTimeOffset? beginDate,
            CancellationToken token);
    }
}