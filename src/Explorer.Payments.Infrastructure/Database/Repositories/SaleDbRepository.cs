using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class SaleDbRepository : ISaleRepository
    {
        private readonly PaymentsContext _context;

        public SaleDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Sale Add(Sale sale)
        {
            _context.Sales.Add(sale);
            _context.SaveChanges();
            return sale;
        }

        public Sale? GetById(long id)
        {
            return _context.Sales
                .Include(s => s.Items)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Sale> GetByTouristId(long touristId)
        {
            return _context.Sales
                .Include(s => s.Items)
                .Where(s => s.TouristId == touristId)
                .ToList();
        }
    }
}
