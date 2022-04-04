using System;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._2_EntityModel;
using Microsoft.EntityFrameworkCore;

namespace AITIssueTracker.API.v0._1_Controller
{
    public class DbTestContext : DbContext
    {
        private readonly PsqlSettings _dbSettings;
        public DbTestContext(PsqlSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_dbSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Test>()
                .Property(t => t.Status)
                .HasConversion(
                    status => status.ToString(),
                    status => (MyType) Enum.Parse(typeof(MyType), status)
                );
            modelBuilder
                .Entity<Test>(builder =>
                {
                    builder.HasKey(testObject => testObject.Name);
                });
        }

        public DbSet<Test> Tests { get; set; }
    }
}