using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AromaAura.Models;

namespace AromaAura.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }  // Add this if not already present

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relationship between Cart and CartItems
            builder.Entity<CartItem>()
                .HasOne(c => c.Cart)
                .WithMany(ci => ci.CartItems)
                .HasForeignKey(c => c.CartId);  // This will link CartItem to Cart (not directly to ApplicationUser)

            // Relationships for Order
            builder.Entity<Order>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(o => o.UserId);

            builder.Entity<OrderItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            builder.Entity<OrderItem>()
                .HasOne<Order>()
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}
