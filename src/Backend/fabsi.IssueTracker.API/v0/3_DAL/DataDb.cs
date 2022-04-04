using System.Collections.Generic;
using AITIssueTracker.Model.v0._2_EntityModel;
using Microsoft.EntityFrameworkCore;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class DataDb : DbContext
    {
        private readonly PsqlSettings _settings;

        public DataDb(PsqlSettings settings)
        {
            _settings = settings;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_settings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>(builder =>
                {
                    builder.HasKey(nameof(User.Username));
                    builder.Property(p => p.Username)
                        .HasMaxLength(10);
                });
        }

        /*
        public DbSet<Project> Projects { get; }

        public DbSet<Feature> Features { get; set; }
        */
    }
}