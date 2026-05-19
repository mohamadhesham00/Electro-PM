using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // If theses configurations were bigger, we could move them to separate configuration classes implementing IEntityTypeConfiguration<T>

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Priority)
                .HasConversion<string>();

        }
    }
}
