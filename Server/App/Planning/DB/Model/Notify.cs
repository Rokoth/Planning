using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("notify")]
    public class Notify : Entity
    {
        [ColumnName("userid")]
        public Guid UserId { get; set; }

        [ColumnName("text")]
        public string Text { get; set; }

        [ColumnName("issend")]
        public bool IsSend { get; set; }
    }
}
