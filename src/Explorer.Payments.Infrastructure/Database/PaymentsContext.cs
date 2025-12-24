
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<TourPurchase> TourPurchases { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        
        public PaymentsContext(DbContextOptions<PaymentsContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // postavljanje default šeme u camelCase
            modelBuilder.HasDefaultSchema("payments");
            modelBuilder.Entity<TourPurchaseToken>(entity =>
            {
                entity.Property(t => t.Status)
                    .HasConversion<string>();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
