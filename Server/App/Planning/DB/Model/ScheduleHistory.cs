using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("h_schedule")]
    public class ScheduleHistory : EntityHistory
    {
        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
        [ColumnName("userid")]
        public Guid UserId { get; set; }       
        [ColumnName("begin_date")]
        public DateTimeOffset BeginDate { get; set; }
        [ColumnName("end_date")]
        public DateTimeOffset EndDate { get; set; }
        [ColumnName("is_running")]
        public bool IsRunning { get; set; }
    }
}
