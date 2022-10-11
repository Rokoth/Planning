using Planning.DB.Context;
using System;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectSelectService
    {
        Task<Schedule> AddProjectToSchedule(Guid userId, UserSettings settings, Guid? projectId = null, DateTimeOffset? beginDate = null, bool setBeginDate = false, bool isLocked = false);
        Task MoveNextSchedule(Guid userId, UserSettings settings);
        Task ShiftSchedule(Guid userId, UserSettings settings, DateTimeOffset now, bool isForce = false, bool isLocked = false);
    }
}