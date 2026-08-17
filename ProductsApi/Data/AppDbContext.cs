using Microsoft.EntityFrameworkCore;
using ProductsApi.Data.Configurations;
using ProductsApi.Models;

namespace ProductsApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
        });

        // TaskItem configuration (required fields, max lengths, defaults, FK to User)
        // is kept in its own IEntityTypeConfiguration class to keep this method clean.
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
    }
}
