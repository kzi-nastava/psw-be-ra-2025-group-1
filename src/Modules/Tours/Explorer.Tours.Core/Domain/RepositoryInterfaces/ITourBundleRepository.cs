using Explorer.Tours.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourBundleRepository
    {
        TourBundle Create(TourBundle bundle);
        TourBundle Update(TourBundle bundle);
        void Delete(long id);
        TourBundle Get(long id);
        List<TourBundle> GetByCreator(long creatorId);
        List<TourBundle> GetAll();
    }
}
