using System;

namespace Planning.DB.Context
{
    public class Project : Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastUsedDate { get; set; }
        public int? Period { get; set; } //In minutes
        public int Priority { get; set; }
        public Guid UserId { get; set; }
        public int AddTime { get; set; }
    }

    public class ProjectHistory : EntityHistory
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastUsedDate { get; set; }
        public int? Period { get; set; } //In minutes
        public int Priority { get; set; }
        public Guid UserId { get; set; }
    }

    public class Schedule : Entity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public int Order { get; set; }
        public DateTimeOffset BeginDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsRunning { get; set; }
        public int ElapsedTime { get; set; }
        public DateTimeOffset StartDate { get; set; }
    }
}
