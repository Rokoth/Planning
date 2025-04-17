using Planning.DB.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planning.DB.Context
{
    [TableName("schedule")]
    public class Schedule : Entity
    {
        [ColumnName("project_id")]       
        public Guid ProjectId { get; set; }
        [ColumnName("userid")]       
        public Guid UserId { get; set; }
        [ColumnName("directionid")]
        public Guid DirectionId { get; set; }
        [ColumnName("begin_date")]
        public DateTimeOffset BeginDate { get; set; }
        [ColumnName("end_date")]
        public DateTimeOffset EndDate { get; set; }
        [ColumnName("is_running")]
        public bool IsRunning { get; set; }
        [ColumnName("add_time")]
        public int? AddTime { get; set; }
        [ColumnName("is_closed")]
        public bool IsClosed { get; set; }

        [ForeignKey("UserId")]
        [Ignore]
        public User User { get; set; }
        [ForeignKey("ProjectId")]
        [Ignore]
        public Project Project { get; set; }        
    }
}
