using System;

namespace Contract.Model
{
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTimeOffset VersionDate { get; set; }
    }

    public class Project : Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsProject { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? LastUsedDate { get; set; }
        public int Period { get; set; } //In minutes
        public int Priority { get; set; }
        public int PriorityActual => Priority + (int)(((DateTimeOffset.Now - (LastUsedDate ?? VersionDate)).TotalMinutes - Period) / Period);
    }

    public class ProjectPeriod : Entity
    {
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public DateTimeOffset? DateBegin { get; set; }
        public DateTimeOffset? DateEnd { get; set; }
        public bool IsClosed { get; set; }
        public Project Project { get; set; }
    }

    public class ProjectFilter
    {
        public bool? IsProject { get; set; }
        public DateTimeOffset? LastUsedDateBegin { get; set; }
        public DateTimeOffset? LastUsedDateEnd { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
