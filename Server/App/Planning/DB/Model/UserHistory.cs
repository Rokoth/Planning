using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("h_user")]
    public class UserHistory : EntityHistory
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("login")]
        public string Login { get; set; }
        [ColumnName("password")]
        public byte[] Password { get; set; }
        [ColumnName("formula_id")]
        public Guid FormulaId { get; set; }
    }
}
