using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.Coupons;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;
using Explorer.Payments.Core.Domain.Sales;  
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        public DbSet<SaleHistory> SalesHistory { get; set; }
        public DbSet<SaleHistoryItem> SaleHistoryItems { get; set; }

        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponRedemption> CouponRedemptions { get; set; }
        public DbSet<Sale> Sales { get; set; }


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

            modelBuilder.Entity<SaleHistory>(builder =>
            {
                builder.HasKey(s => s.Id);
                builder.Property(s => s.TouristId).IsRequired();
                builder.Property(s => s.PurchasedAt).IsRequired();
                builder.Property(s => s.TotalPrice).IsRequired();

                builder.HasMany(s => s.Items)
                       .WithOne()
                       .OnDelete(DeleteBehavior.Cascade);
            });

            // SaleItem mapping
            modelBuilder.Entity<SaleHistoryItem>(builder =>
            {
                builder.HasKey(si => si.Id);
                builder.Property(si => si.TourId).IsRequired();
                builder.Property(si => si.TourName).IsRequired();
                builder.Property(si => si.Price).IsRequired();
                builder.Property(si => si.Quantity).IsRequired();
            });
           
            modelBuilder.ApplyConfiguration(new SaleConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}