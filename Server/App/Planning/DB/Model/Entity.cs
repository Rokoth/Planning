using System;

namespace DB.Context
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset VersionDate { get; set; }
    }
}
