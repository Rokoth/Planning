using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
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
}
