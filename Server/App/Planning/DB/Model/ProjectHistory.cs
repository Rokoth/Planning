using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("h_project")]
    public class ProjectHistory : EntityHistory
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
    }
}
