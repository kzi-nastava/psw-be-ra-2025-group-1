using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
{
    private readonly PaymentsContext _context;

    public TourPurchaseTokenDbRepository(PaymentsContext context)
    {
        _context = context;
    }

    public TourPurchaseToken Create(TourPurchaseToken token)
    {
        _context.TourPurchaseTokens.Add(token);
        _context.SaveChanges();
        return token;
    }

    public TourPurchaseToken? Get(long id)
    {
        return _context.TourPurchaseTokens.FirstOrDefault(t => t.Id == id);
    }

    public List<TourPurchaseToken> GetByUserId(long userId)
    {
        return _context.TourPurchaseTokens
            .Where(t => t.UserId == userId)
            .ToList();
    }

    public bool ExistsForUserAndTour(long userId, long tourId)
    {
        return _context.TourPurchaseTokens
            .Any(t => t.UserId == userId && t.TourId == tourId);
    }

    public TourPurchaseToken Update(TourPurchaseToken token)
    {
        _context.TourPurchaseTokens.Update(token);
        _context.SaveChanges();
        return token;
    }
}