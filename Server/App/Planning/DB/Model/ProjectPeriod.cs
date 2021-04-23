using System;

namespace DB.Context
{
    public class ProjectPeriod : Entity
    {
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public DateTimeOffset? DateBegin { get; set; }
        public DateTimeOffset? DateEnd { get; set; }
        public bool IsClosed { get; set; }
        public Project Project { get; set; }
    }
}
