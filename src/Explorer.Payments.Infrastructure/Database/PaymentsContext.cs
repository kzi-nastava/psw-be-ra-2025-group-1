using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.Bundles;
using Explorer.Payments.Core.Domain.Coupons;
using Explorer.Payments.Core.Domain.Sales;
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
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponRedemption> CouponRedemptions { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<BundlePurchase> BundlePurchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponRedemption> CouponRedemptions { get; set; }
        
        public PaymentsContext(DbContextOptions<PaymentsContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("payments");
            
            modelBuilder.Entity<TourPurchaseToken>(entity =>
            {
                entity.Property(t => t.Status)
                    .HasConversion<string>();
            });
            
            modelBuilder.Entity<Bundle>(entity =>
            {
                entity.Property(b => b.Status)
                    .HasConversion<string>();
                    
                entity.Property(b => b.TourIds)
                    .HasColumnType("bigint[]");
            });
            
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(s => s.TourIds)
                    .HasColumnType("bigint[]");
            });
            
            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.HasIndex(c => c.Code).IsUnique();
                
                entity.Property(c => c.DiscountType)
                    .HasConversion<string>();
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}