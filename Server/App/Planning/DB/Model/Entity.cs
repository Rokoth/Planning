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
    }

    public abstract class EntityHistory : IEntity
    {
        [PrimaryKey]
        [ColumnName("h_id")]
        public long HId { get; set; }
        [ColumnName("change_date")]
        public DateTimeOffset ChangeDate { get; set; }

        [ColumnName("id")]
        public Guid Id { get; set; }
        [ColumnName("version_date")]
        public DateTimeOffset VersionDate { get; set; }
        [ColumnName("is_deleted")]
        public bool IsDeleted { get; set; }
    }

    public interface IEntity
    {
        Guid Id { get; set; }
        bool IsDeleted { get; set; }
        DateTimeOffset VersionDate { get; set; }
    }
}
