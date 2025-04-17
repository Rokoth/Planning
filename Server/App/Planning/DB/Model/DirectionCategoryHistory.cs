using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
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
}
