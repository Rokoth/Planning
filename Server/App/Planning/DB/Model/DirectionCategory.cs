using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
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
}
