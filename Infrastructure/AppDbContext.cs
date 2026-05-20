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

            // seed some admin, user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("B74A3D90-7F89-4D2A-8B6C-773C33D252E1"),
                    Email = "user@example.com",
                    Password = "$2a$12$v7QEJTY67SZKKP0g94O/s.Q8A0HLLHC5HLbcMCGUGGeDUcx3/HeZa",
                    Role = Domain.Enums.UserRole.User
                },
                new User
                {
                    Id = Guid.Parse("A12B4E91-1A22-4B1C-BD6E-884D44E363F2"),
                    Email = "admin@example.com",
                    Password = "$2a$12$v7QEJTY67SZKKP0g94O/s.Q8A0HLLHC5HLbcMCGUGGeDUcx3/HeZa",
                    Role = Domain.Enums.UserRole.Admin
                }
            );

        }
    }
}
