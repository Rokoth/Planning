using System;
using System.Linq.Expressions;

namespace Planning.Service
{
    public class UserSettingsDataService : DataService<DB.Context.UserSettings, Contract.Model.UserSettings,
       Contract.Model.UserSettingsFilter, Contract.Model.UserSettingsCreator, Contract.Model.UserSettingsUpdater>
    {
        public UserSettingsDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<DB.Context.UserSettings, bool>> GetFilter(Contract.Model.UserSettingsFilter filter)
        {
            return s => s.UserId == filter.UserId;
        }        
        protected override DB.Context.UserSettings UpdateFillFields(Contract.Model.UserSettingsUpdater entity, DB.Context.UserSettings entry)
        {
            entry.ScheduleCount = entity.ScheduleCount;
            entry.DefaultProjectTimespan = entity.DefaultProjectTimespan;
            entry.LeafOnly = entity.LeafOnly;
            entry.ScheduleMode = entity.ScheduleMode;
            entry.ScheduleShift = entity.ScheduleShift;
            entry.ScheduleTimeSpan = entity.ScheduleTimeSpan;
            return entry;
        }

        protected override string DefaultSort => "Name";

    }
}
