using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Migrator
{    
    public class MigrateContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFirebird(@"database=localhost:F:\dbs\PROJECTS.FDB;user=sysdba;password=Rok_Oth_123");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MigrateProject>().ToTable("PROJS");
        }
    }

    public class MigrateProject
    {
        public int? ID { get; set; }
        public int? LVL { get; set; }
        public string PRJT { get; set; }
        public int? PARENT { get; set; }
        public int? ENDPOINT { get; set; }
        public int? PRIO { get; set; }
        public int? PR_LEVEL { get; set; }
        public int? TIMINGS { get; set; }
        public string LAST_USE { get; set; }
        public int? TIMS { get; set; }
        public int? DDF { get; set; }
        public string OUTPROJS { get; set; }
    }
}
