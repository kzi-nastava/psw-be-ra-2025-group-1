// Explorer.Tours.Core/UseCases/TourInfoService.cs
using Explorer.BuildingBlocks.Core.Services;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class TourInfoService : ITourInfoService
    {
        private readonly ITourRepository _tourRepo;

        public TourInfoService(ITourRepository tourRepo)
        {
            _tourRepo = tourRepo;
        }

        public TourInfoDto? GetPublishedTourInfo(long tourId)
        {
            var tour = _tourRepo.GetPublishedById(tourId);
            if (tour == null) return null;

            return MapToDto(tour);
        }

        public TourInfoDto? GetTourInfo(long tourId)
        {
            var tour = _tourRepo.Get(tourId);
            if (tour == null) return null;

            return MapToDto(tour);
        }

        private TourInfoDto MapToDto(Tour tour)
        {
            return new TourInfoDto
            {
                Id = tour.Id,
                Title = tour.Title,
                Price = tour.Price,
                Status = tour.Status.ToString()
            };
        }
    }
}