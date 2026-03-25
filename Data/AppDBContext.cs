using backend_purchase_order.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Order> Order { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<ItemOrder> ItemOrder { get; set; }
    public DbSet<OrderActionHistory> OrderActionHistory { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemOrder>()
            .HasIndex(io => new { io.ItemId, io.OrderId })
            .IsUnique();

        modelBuilder.Entity<OrderActionHistory>()
            .HasIndex(io => new { io.UserId, io.OrderId })
            .IsUnique();

        modelBuilder.Entity<OrderActionHistory>()
            .HasOne(oah => oah.Order)
            .WithMany()
            .HasForeignKey(oah => oah.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}