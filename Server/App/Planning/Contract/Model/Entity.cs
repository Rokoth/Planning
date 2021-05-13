

using System;

namespace Planning.Contract.Model
{
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTimeOffset VersionDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
