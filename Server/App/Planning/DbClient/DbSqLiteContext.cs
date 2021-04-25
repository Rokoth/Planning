using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Planning.DbClient
{
    public class DbSqLiteContext : DbContext
    {
        //public DbSet<Tree> Trees { get; set; }
        //public DbSet<TreeItem> TreeItems { get; set; }
        //public DbSet<Formula> Formulas { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public DbSqLiteContext(DbContextOptions<DbSqLiteContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new TreeEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new TreeItemEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new FormulaEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SettingsEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }

    public class SettingsEntityConfiguration : IEntityTypeConfiguration<Settings>
    {
        public void Configure(EntityTypeBuilder<Settings> builder)
        {            
            builder.ToTable("settings");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ParamName).HasColumnName("param_name");
            builder.Property(s => s.ParamValue).HasColumnName("param_value");
        }
    }

    public class Settings
    {
        public int Id { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }
    }
}
