using Planning.DB.Attributes;
using System.Collections.Generic;

namespace Planning.DB.Context
{
    [TableName("formula")]
    public class Formula : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("text")]
        public string Text { get; set; }

        [ColumnName("is_default")]
        public bool IsDefault { get; set; }

        [Ignore]
        public List<User> Users { get; set; }
    }
}
