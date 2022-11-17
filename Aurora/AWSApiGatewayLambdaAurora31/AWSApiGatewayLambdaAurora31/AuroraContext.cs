using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSApiGatewayLambdaAurora31
{

    public class AuroraContext : DbContext
    {
        private readonly string _connectionString;

        public AuroraContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");
                entity.HasKey(e => e.Id); //primary key designator
            });
        }
    }
}
