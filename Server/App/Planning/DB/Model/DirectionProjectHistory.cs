using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("h_direction_project")]
    public class DirectionProjectHistory : EntityHistory
    {
        [ColumnName("direction_id")]
        public Guid DirectionId { get; set; }

        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
    }
}
