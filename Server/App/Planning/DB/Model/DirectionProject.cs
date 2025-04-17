using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("direction_project")]
    public class DirectionProject : Entity
    {       
        [ColumnName("direction_id")]
        public Guid DirectionId { get; set; }

        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
    }
}
