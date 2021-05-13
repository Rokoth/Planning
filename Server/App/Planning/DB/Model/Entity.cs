using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset VersionDate { get; set; }
    }

    [TableName("settings")]
    public class Settings
    {
        [ColumnName("id")]
        public int Id { get; set; }
        [ColumnName("param_name")]
        public string ParamName { get; set; }
        [ColumnName("param_value")]
        public string ParamValue { get; set; }
    }
}
