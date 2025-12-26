using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class SaleHistoryDbRepository : ISaleHistoryRepository
    {
        private readonly PaymentsContext _context;

        public SaleHistoryDbRepository(PaymentsContext context)
        {
            _context = context;
        }

        public SaleHistory Add(SaleHistory sale)
        {
            _context.SalesHistory.Add(sale);
            _context.SaveChanges();
            return sale;
        }

        public SaleHistory? GetById(long id)
        {
            return _context.SalesHistory
                .Include(s => s.Items)
                .FirstOrDefault(s => s.Id == id);
        }

        public List<SaleHistory> GetByTouristId(long touristId)
        {
            return _context.SalesHistory
                .Include(s => s.Items)
                .Where(s => s.TouristId == touristId)
                .ToList();
        }
    }
}
