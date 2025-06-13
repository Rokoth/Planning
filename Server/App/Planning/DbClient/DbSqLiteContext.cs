//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Microsoft.EntityFrameworkCore;

namespace Planning.DbClient
{
    public class DbSqLiteContext : DbContext
    {       
        public DbSet<Settings> Settings { get; set; }

        public DbSqLiteContext(DbContextOptions<DbSqLiteContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.ApplyConfiguration(new SettingsEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
