using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planning.DB.Attributes;
using System;
using System.Reflection;

namespace Planning.DB.Context
{
    public class EntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            SetTableName(builder, typeof(T));

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!SetIgnore(builder, prop))
                {
                    SetPrimaryKey(builder, prop);
                    SetColumnName(builder, prop);
                    SetColumnType(builder, prop);                    
                }
            }
        }

        private bool SetIgnore(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var ignore = prop.GetCustomAttribute<IgnoreAttribute>();
            if (ignore != null)
            {
                builder.Ignore(prop.Name);
                return true;
            }
            return false;
        }

        private void SetColumnType(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var ctAttr = prop.GetCustomAttribute<ColumnTypeAttribute>();
            if (ctAttr != null)
            {
                builder.Property(prop.Name).HasColumnType(ctAttr.Name);
            }
        }

        private void SetColumnName(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var propAttribute = prop.GetCustomAttribute<ColumnNameAttribute>();
            if (propAttribute != null)
                builder.Property(prop.Name)
                    .HasColumnName(propAttribute.Name);
            else
                builder.Property(prop.Name)
                    .HasColumnName(prop.Name);
        }                

        private void SetPrimaryKey(EntityTypeBuilder<T> builder, PropertyInfo prop)
        {
            var pkAttr = prop.GetCustomAttribute<PrimaryKeyAttribute>();
            if (pkAttr != null)
            {
                builder.HasKey(prop.Name);
            }
        }

        private void SetTableName(EntityTypeBuilder<T> builder, Type type)
        {
            var typeAttribute = type.GetCustomAttribute<TableNameAttribute>();
            if (typeAttribute != null)
            {
                builder.ToTable(typeAttribute.Name);
            }
            else
            {
                builder.ToTable(type.Name);
            }
        }
    }

    //public class MigrateContext : DbContext
    //{

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseFirebird(@"database=localhost:projects.fdb;user=sysdba;password=Rash_Idio_123");
    //        base.OnConfiguring(optionsBuilder);
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //    }
    //}

    //public class MigrateProject
    //{
    //    public int Id { get; set; }
    //    public int Lvl { get; set; }
    //    public string Prjt { get; set; }
    //    public int Parent { get; set; }
    //    public int Endpoint { get; set; }
    //    public int Prio { get; set; }
    //    public int PrLvl { get; set; }
    //    public int Timings { get; set; }
    //    public string LastUse { get; set; }
    //    public int Tims { get; set; }
    //    public int Ddf { get; set; }
    //    public string OutProjs { get; set; }
    //}
}
