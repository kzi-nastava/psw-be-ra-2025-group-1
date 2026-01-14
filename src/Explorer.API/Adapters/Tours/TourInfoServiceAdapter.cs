using Explorer.Payments.Core.Domain.External;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.API.Adapters.Tours
{
    public class TourInfoServiceAdapter : ITourInfoService
    {
        private readonly ITourRepository _tourRepo;

        public TourInfoServiceAdapter(ITourRepository tourRepo)
        {
            _tourRepo = tourRepo;
        }

        public TourInfo GetById(long tourId)
        {
            var t = _tourRepo.Get(tourId);
            if (t == null) throw new ArgumentException("Tour not found.");

            return new TourInfo(
                t.Id,
                t.CreatorId,
                t.Title,
                (decimal)t.Price,
                MapStatus(t.Status)
            );
        }

        private static TourPublishStatus MapStatus(TourStatus status)
            => status switch
            {
                TourStatus.Published => TourPublishStatus.Published,
                TourStatus.Archived  => TourPublishStatus.Archived,
                _                    => TourPublishStatus.Draft
            };
    }
}