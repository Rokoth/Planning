using Contracts.Model.Schedule;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectSelectService
    {
        Task<Schedule> AddProjectToSchedule(Guid userId, 
            Guid? projectId = null, 
            Guid? directionIdId = null, 
            DateTimeOffset? beginDate = null, 
            bool setBeginDate = false, 
            bool isLocked = false);

        Task<IEnumerable<Schedule>> GetNextShedules(
            Guid userId,
            int count,
            DateTime? beginDate,
            CancellationToken token);
    }
}