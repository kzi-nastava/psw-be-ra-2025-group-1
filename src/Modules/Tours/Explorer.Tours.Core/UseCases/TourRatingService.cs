using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases
{
    public class TourRatingService : ITourRatingService
    {
        private readonly ITourRatingRepository _tourRatingRepository;
        private readonly ITourExecutionRepository _tourExecutionRepository;
        private readonly IMapper _mapper;

        public TourRatingService(ITourRatingRepository tourRatingRepository, IMapper mapper, ITourExecutionRepository tourExecutionRepository)
        {
            _tourRatingRepository = tourRatingRepository;
            _tourExecutionRepository = tourExecutionRepository;
            _mapper = mapper;
        }

        public TourRatingDto Get(long id)
        {
            var result = _tourRatingRepository.Get(id);
            return _mapper.Map<TourRatingDto>(result);
        }

        public PagedResult<TourRatingDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByUser(long userId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPagedByUser(userId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByTourExecution(long tourExecutionId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPagedByTourExecution(tourExecutionId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public TourRatingDto Create(TourRatingDto rating)
        {
            // Check if the execution exists
            var execution = _tourExecutionRepository.Get(rating.TourExecutionId); // This will throw NotFoundException if not found
            // Check if this is the user that has completed that tour
            if (execution.TouristId != rating.UserId) throw new System.UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");
            // Check if the user has completed the tour
            if (execution.Status != TourExecutionStatus.Completed) throw new System.InvalidOperationException("Tour needs to be completed in order to rate it.");
            // Check if the user has completed the tour
            if (execution.PercentageCompleted < 50) throw new System.InvalidOperationException("Tour needs to be completed at least 50% in order to rate it.");
            // Check if the values are okay
            if (rating.Stars < 1 || rating.Stars > 5) throw new System.ArgumentException("Stars must be between 1 and 5.");
            // Check that it hasn't passed more than 3 months since last activity
            if(execution.LastActivity.AddMonths(3) < System.DateTime.UtcNow) throw new System.ArgumentException("Cannot rate tour after 3 months since last activity.");
            // Check if the user has already rated this tour execution
            if (_tourRatingRepository.GetPagedByUser(rating.UserId, 1, int.MaxValue).Results.Any(r => r.TourExecutionId == rating.TourExecutionId)) throw new System.InvalidOperationException("You have already rated this tour execution.");

            rating.Id = 0;
            rating.CompletedProcentage = execution.PercentageCompleted;
            rating.ThumbsUpCount = 0;

            var result = _tourRatingRepository.Create(_mapper.Map<TourRating>(rating));
            return _mapper.Map<TourRatingDto>(result);
        }

        public TourRatingDto Update(long id, TourRatingDto rating)
        {
            // Get entity directly from repository
            var existingRating = _tourRatingRepository.Get(id); // This will throw NotFoundException if not found

            if (existingRating.UserId != rating.UserId)
                throw new UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");

            // Update on tracked entity
            existingRating.UpdateRating(rating.Comment, rating.Stars);

            var result = _tourRatingRepository.Update(existingRating);
            return _mapper.Map<TourRatingDto>(result);
        }

        public void Delete(long id, long userId)
        {
            // Check if the rating exists
            var existingRating = Get(id); // This will throw NotFoundException if not found
            // Check if the rating belongs to the user
            if (existingRating.UserId != userId) throw new System.UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");

            _tourRatingRepository.Delete(id);
        }
    }
}