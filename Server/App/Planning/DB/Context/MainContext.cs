using Microsoft.EntityFrameworkCore;

namespace DB.Context
{
    public class MainContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Database=project;Username=postgres;Password=postgres");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProjetConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPeriodConfiguration());
        }
    }

    public class MigrateContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFirebird(@"database=localhost:projects.fdb;user=sysdba;password=Rash_Idio_123");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class MigrateProject
    {
        public int Id { get; set; }
        public int Lvl { get; set; }
        public string Prjt { get; set; }
        public int Parent { get; set; }
        public int Endpoint { get; set; }
        public int Prio { get; set; }
        public int PrLvl { get; set; }
        public int Timings { get; set; }
        public string LastUse { get; set; }
        public int Tims { get; set; }
        public int Ddf { get; set; }
        public string OutProjs { get; set; }
    }
}
