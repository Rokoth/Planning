using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("h_user_settings")]
    public class UserSettingsHistory : EntityHistory
    {
        [ColumnName("userid")]
        public Guid UserId { get; set; }
        [ColumnName("schedule_mode")]
        public Contract.Model.ScheduleMode ScheduleMode { get; set; }
        [ColumnName("schedule_count")]
        public int? ScheduleCount { get; set; }
        [ColumnName("schedule_timespan")]
        public int? ScheduleTimeSpan { get; set; } // hours
        [ColumnName("default_project_timespan")]
        public int DefaultProjectTimespan { get; set; }
        [ColumnName("leaf_only")]
        public bool LeafOnly { get; set; }
        [ColumnName("schedule_shift")]
        public int ScheduleShift { get; set; }
    }
}
