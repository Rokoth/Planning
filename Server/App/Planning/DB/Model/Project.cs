using Planning.DB.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planning.DB.Context
{
    [TableName("project")]
    public class Project : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("path")]
        public string Path { get; set; }
        [ColumnName("parent_id")]
        public Guid? ParentId { get; set; }
        [ColumnName("is_leaf")]
        public bool IsLeaf { get; set; }
        [ColumnName("last_used_date")]
        public DateTimeOffset? LastUsedDate { get; set; }
        [ColumnName("period")]
        public int? Period { get; set; } //In minutes
        [ColumnName("priority")]
        public int Priority { get; set; }
        [ColumnName("userid")]        
        public Guid UserId { get; set; }
        [ColumnName("add_time")]
        public int AddTime { get; set; }


        [ForeignKey("ParentId")]
        [Ignore]
        public Project Parent { get; set; }
        [ForeignKey("UserId")]
        [Ignore]
        public User User { get; set; }

        [Ignore]
        public List<Schedule> Schedules { get; set; }
    }
}
