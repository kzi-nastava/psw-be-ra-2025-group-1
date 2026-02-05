using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Payments.Tests
{
    public class BasePaymentsIntegrationTest:BaseWebIntegrationTest<PaymentsTestFactory>
    {
        public BasePaymentsIntegrationTest(PaymentsTestFactory factory): base(factory){}

        /// <summary>
        /// Ensures test tours exist in the database. Uses INSERT ON CONFLICT to handle
        /// race conditions when other test assemblies wipe the tours table.
        /// Call this in the Arrange section of any test that depends on tour data.
        /// </summary>
        protected void EnsureTourTestData(IServiceScope scope)
        {
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            toursContext.Database.ExecuteSqlRaw(@"
                INSERT INTO tours.""Tour""(
                    ""Id"", ""CreatorId"", ""Title"", ""Description"", ""Difficulty"",
                    ""Tags"", ""Status"", ""Price"", ""CreatedAt"", ""UpdatedAt"",
                    ""PublishedAt"", ""ArchivedAt"")
                OVERRIDING SYSTEM VALUE
                VALUES
                    (1, -11, 'Test Tour 1', 'Tour for testing', 2, ARRAY['test'], 1, 15.0, '2026-01-01', '2026-01-01', '2026-01-01', '-infinity'),
                    (2, -11, 'Test Tour 2', 'Tour for testing', 3, ARRAY['test'], 1, 20.0, '2026-01-01', '2026-01-01', '2026-01-01', '-infinity'),
                    (6, -11, 'Test Tour 6', 'Tour for testing', 1, ARRAY['test'], 1, 10.0, '2026-01-01', '2026-01-01', '2026-01-01', '-infinity')
                ON CONFLICT (""Id"") DO NOTHING;
            ");
        }
    }
}
