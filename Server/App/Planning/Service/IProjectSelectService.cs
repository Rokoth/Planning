using System;
using System.Threading.Tasks;

namespace Planning.Service
{
    public interface IProjectSelectService
    {
        Task<Contracts.Model.Schedule> AddProjectToSchedule(Guid userId, DB.Context.UserSettings settings, Guid? projectId = null, DateTimeOffset? beginDate = null, bool setBeginDate = false, bool isLocked = false);
        Task MoveNextSchedule(Guid userId, DB.Context.UserSettings settings);
        Task ShiftSchedule(Guid userId, DB.Context.UserSettings settings, DateTimeOffset now, bool isForce = false, bool isLocked = false);
    }
}