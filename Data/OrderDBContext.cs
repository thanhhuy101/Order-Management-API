using Microsoft.EntityFrameworkCore;
using Order_Management.Entities;

namespace Order_Management.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; } = null;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure Order entity
            modelBuilder.Entity<Order>()
                .Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            //Configure OrderDetail entity
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.ProductName)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");

            //Configure relationship
            modelBuilder.Entity<OrderDetail>()
               .HasOne(od => od.Order)
               .WithMany(o => o.OrderDetails)
               .HasForeignKey(od => od.OrderId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
