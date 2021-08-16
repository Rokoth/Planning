//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planning.DB.Context
{
    /// <summary>
    /// Context for connect to postgres
    /// </summary>
    public class DbPgContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<FormulaHistory> FormulaHistories { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectHistory> ProjectHistories { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<ScheduleHistory> ScheduleHistories { get; set; }
        public DbSet<UserSettingsHistory> UserSettingsHistories { get; set; }

        public DbPgContext(DbContextOptions<DbPgContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.ApplyConfiguration(new EntityConfiguration<Settings>());

            foreach (var type in Assembly.GetAssembly(typeof(Entity)).GetTypes())
            {
                if (typeof(IEntity).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var config = Activator.CreateInstance(typeof(EntityConfiguration<>).MakeGenericType(type));
                    GetType().GetMethod(nameof(ApplyConf), BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(type).Invoke(this, new object[] { modelBuilder, config });
                }
            }
        }

        private void ApplyConf<T>(ModelBuilder modelBuilder, EntityConfiguration<T> config) where T : class, IEntity
        {
            modelBuilder.ApplyConfiguration(config);
        }
    }
}
