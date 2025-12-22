
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public PaymentsContext(DbContextOptions<PaymentsContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // postavljanje default šeme u camelCase
            modelBuilder.HasDefaultSchema("payments");

            base.OnModelCreating(modelBuilder);
        }
    }
}
