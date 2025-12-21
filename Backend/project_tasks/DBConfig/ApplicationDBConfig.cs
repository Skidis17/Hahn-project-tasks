using Microsoft.EntityFrameworkCore;
using Project_tasks.Models;

// This file contains the database configuration for the application

namespace Project_tasks.DBConfig
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TodoItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // In here we configure the entity relationships and constraints of our models 

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasMany(u => u.Projects)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.IdUser)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Project 
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasMany(p => p.Tasks)
                    .WithOne(t => t.Project)
                    .HasForeignKey(t => t.IdProject)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Task 
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.Property(t => t.Status)
                    .HasConversion<string>();
            });
        }
    }
}