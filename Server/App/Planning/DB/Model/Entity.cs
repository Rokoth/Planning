using Planning.DB.Attributes;
using System;
using System.Linq.Expressions;

namespace Planning.DB.Context
{
    public abstract class Entity : IEntity
    {
        [ColumnName("id")]
        public Guid Id { get; set; }
        [ColumnName("is_deleted")]
        public bool IsDeleted { get; set; }
        [ColumnName("version_date")]
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
        [ColumnName("formula_id")]
        public Guid FormulaId { get; set; }
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

    [TableName("user")]
    public class User : Entity, IIdentity
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

    public interface IIdentity
    {
        string Login { get; set; }
        byte[] Password { get; set; }
    }

    public interface IEntity
    {
        Guid Id { get; set; }
        bool IsDeleted { get; set; }
        DateTimeOffset VersionDate { get; set; }
    }

    public class Filter<T> where T : IEntity
    {
        public int? Page { get; set; }
        public int? Size { get; set; }
        public string Sort { get; set; }

        public Expression<Func<T, bool>> Selector { get; set; }
    }

    [TableName("formula")]
    public class Formula : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("text")]
        public string Text { get; set; }

        [ColumnName("is_default")]
        public bool IsDefault { get; set; }
    }

    [TableName("h_formula")]
    public class FormulaHistory : EntityHistory
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("text")]
        public string Text { get; set; }

        [ColumnName("is_default")]
        public bool IsDefault { get; set; }
    }
}
