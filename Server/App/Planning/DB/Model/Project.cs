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

    [TableName("direction_category")]
    public class DirectionCategory : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("priority")]
        public int Priority { get; set; }               
    }

    [TableName("h_direction_category")]
    public class DirectionCategoryHistory : EntityHistory
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("priority")]
        public int Priority { get; set; }
    }

    [TableName("direction")]
    public class Direction : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("direction_category_id")]
        public Guid DirectionCategoryId { get; set; }

        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("priority")]
        public int Priority { get; set; }

        [ColumnName("begin_date")]
        public DateTime? BeginDate { get; set; }
    }

    [TableName("h_direction")]
    public class DirectionHistory : EntityHistory
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("description")]
        public string Description { get; set; }

        [ColumnName("direction_category_id")]
        public Guid DirectionCategoryId { get; set; }

        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("priority")]
        public int Priority { get; set; }

        [ColumnName("begin_date")]
        public DateTime? BeginDate { get; set; }
    }

    [TableName("direction_project")]
    public class DirectionProject : Entity
    {       
        [ColumnName("direction_id")]
        public Guid DirectionId { get; set; }

        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
    }

    [TableName("h_direction_project")]
    public class DirectionProjectHistory : EntityHistory
    {
        [ColumnName("direction_id")]
        public Guid DirectionId { get; set; }

        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
    }
}
