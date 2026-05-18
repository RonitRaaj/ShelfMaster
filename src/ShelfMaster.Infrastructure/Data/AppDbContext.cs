using Microsoft.EntityFrameworkCore;
using ShelfMaster.Domain.Entities;
namespace ShelfMaster.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<User> Users { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.LowStockThreshold).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(36);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
        });
    }
}