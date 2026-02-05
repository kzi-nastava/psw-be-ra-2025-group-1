using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.Cupons;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;
using Explorer.Payments.Core.Domain.Sales;
using Explorer.Payments.Core.Domain.Bundles;
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

        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default schema
            modelBuilder.HasDefaultSchema("payments");

            // Configure tables
            modelBuilder.Entity<ShoppingCart>().ToTable("ShoppingCarts");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
            modelBuilder.Entity<TourPurchase>().ToTable("TourPurchases");
            modelBuilder.Entity<Coupon>().ToTable("Coupons");
            modelBuilder.Entity<CouponRedemption>().ToTable("CouponRedemptions");
            modelBuilder.Entity<TourPurchaseToken>().ToTable("TourPurchaseTokens");
            modelBuilder.Entity<Sale>().ToTable("Sales");
            modelBuilder.Entity<Wallet>().ToTable("Wallets");
            modelBuilder.Entity<Bundle>().ToTable("Bundles");
            modelBuilder.Entity<BundlePurchase>().ToTable("BundlePurchases");

            // Enum conversions
            modelBuilder.Entity<TourPurchaseToken>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Bundle>()
                .Property(b => b.Status)
                .HasConversion<string>();

            // Apply configurations
            modelBuilder.ApplyConfiguration(new SaleConfiguration());

            // Configure relationships
            ConfigureShoppingCart(modelBuilder);
            ConfigureTourPurchases(modelBuilder);
        }

        private static void ConfigureShoppingCart(ModelBuilder modelBuilder)
        {
            // ShoppingCart → OrderItems relationship
            modelBuilder.Entity<ShoppingCart>()
                .HasMany(sc => sc.Items)
                .WithOne()
                .HasForeignKey("ShoppingCartId")
                .OnDelete(DeleteBehavior.Cascade);
        }

        private static void ConfigureTourPurchases(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TourPurchase>(cfg =>
            {
                cfg.HasKey(tp => tp.Id);
                cfg.Property(tp => tp.TouristId).IsRequired();
                cfg.Property(tp => tp.TourId).IsRequired();
                cfg.Property(tp => tp.PricePaid).IsRequired().HasColumnType("decimal(18,2)");
                cfg.Property(tp => tp.PurchaseDate).IsRequired();

                cfg.HasIndex(tp => tp.TouristId);
                cfg.HasIndex(tp => tp.TourId);
                cfg.HasIndex(tp => tp.PurchaseDate);
            });
        }
    }
}
