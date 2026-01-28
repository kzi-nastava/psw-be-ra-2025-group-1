using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
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
        private readonly ITourRatingReactionRepository _tourReactionRepository;
        private readonly ITouristStatsRepository _touristStatsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TourRatingService(
            ITourRatingRepository tourRatingRepository,
            ITourExecutionRepository tourExecutionRepository,
            ITourRatingReactionRepository tourReactionRepository,
            ITouristStatsRepository touristStatsRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _tourRatingRepository = tourRatingRepository;
            _tourExecutionRepository = tourExecutionRepository;
            _tourReactionRepository = tourReactionRepository;
            _touristStatsRepository = touristStatsRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public TourRatingDto Get(long id, long userId)
        {
            var rating = _tourRatingRepository.Get(id);
            var dto = _mapper.Map<TourRatingDto>(rating);

            ApplyExtraInfo((IEnumerable<TourRatingDto>)dto, userId);

            return dto;
        }


        public PagedResult<TourRatingDto> GetPaged(long userId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            ApplyExtraInfo(items, userId);
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByUser(long userId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPagedByUser(userId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            ApplyExtraInfo(items, userId);
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByTour(long tourId, long userId, int page, int pageSize)
        {
            var executions = _tourExecutionRepository.GetByTourId(tourId);
            if(executions.Count == 0)
            {
                throw new NotFoundException("Tour not found or has no executions.");
            }
            var result = new List<TourRatingDto>();
            foreach (var execution in executions)
                {
                var ratings = _tourRatingRepository.GetPagedByTourExecution(execution.Id, 1, int.MaxValue);
                result.AddRange(ratings.Results.Select(_mapper.Map<TourRatingDto>));
            }
            ApplyExtraInfo(result, userId);

            return new PagedResult<TourRatingDto>(result.Skip((page - 1) * pageSize).Take(pageSize).ToList(), result.Count);
        }

        public TourRatingDto Create(TourRatingDto rating)
        {
            // Check if the execution exists
            var execution = _tourExecutionRepository.Get(rating.TourExecutionId); // This will throw NotFoundException if not found
            // Check if this is the user that has completed that tour
            if (execution.TouristId != rating.UserId) throw new System.UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");
            // Check if the user has completed at least 35% of the tour
            if (execution.PercentageCompleted < 35) throw new System.InvalidOperationException("Tour needs to be completed at least 50% in order to rate it.");
            // Check if the values are okay
            if (rating.Stars < 1 || rating.Stars > 5) throw new System.ArgumentException("Stars must be between 1 and 5.");
            // Check that it hasn't passed more than 1 week since last activity
            if (execution.LastActivity.AddDays(7) < System.DateTime.UtcNow) throw new System.ArgumentException("Cannot rate tour after 1 week since last activity.");
            // Check if the user has already rated this tour execution
            if (_tourRatingRepository.GetPagedByUser(rating.UserId, 1, int.MaxValue).Results.Any(r => r.TourExecutionId == rating.TourExecutionId)) throw new System.InvalidOperationException("You have already rated this tour execution.");

            rating.Id = 0;
            rating.CompletedProcentage = execution.PercentageCompleted;
            rating.ThumbsUpCount = 0;

            var result = _tourRatingRepository.Create(_mapper.Map<TourRating>(rating));

            // Update tourist stats
            _touristStatsRepository.AddRating(rating.UserId);

            return _mapper.Map<TourRatingDto>(result);
        }

        public TourRatingDto Update(long id, TourRatingDto rating)
        {
            // Get entity directly from repository
            var existingRating = _tourRatingRepository.Get(id); // This will throw NotFoundException if not found

            if (existingRating.UserId != rating.UserId)
                throw new UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");

            var execution = _tourExecutionRepository.Get(rating.TourExecutionId); // This will throw NotFoundException if not found

            // Check if this is the user that has completed that tour
            if (execution.TouristId != rating.UserId) throw new System.UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");
            // Check if the values are okay
            if (rating.Stars < 1 || rating.Stars > 5) throw new System.ArgumentException("Stars must be between 1 and 5.");
            // Check that it hasn't passed more than 1 week since last activity
            if (execution.LastActivity.AddDays(7) < System.DateTime.UtcNow) throw new System.ArgumentException("Cannot rate tour after 1 week since last activity.");

            // Update on tracked entity
            existingRating.UpdateRating(rating.Comment, rating.Stars);

            var result = _tourRatingRepository.Update(existingRating);

            var dto = _mapper.Map<TourRatingDto>(result);
            dto.IsThumbedUpByCurrentUser =
                _tourReactionRepository.Exists(dto.Id, rating.UserId);
            return dto;
        }

        public void Delete(long id, long userId)
        {
            // Check if the rating exists
            var existingRating = Get(id, userId); // This will throw NotFoundException if not found
            // Check if the rating belongs to the user
            if (existingRating.UserId != userId) throw new System.UnauthorizedAccessException("Invalid tour ID. This tour belongs to someone else.");

            _tourRatingRepository.Delete(id);

            _touristStatsRepository.RemoveRating(userId);
        }

        private void ApplyExtraInfo(
            IEnumerable<TourRatingDto> ratings,
            long currentUserId)
        {
            foreach (var rating in ratings)
            {
                rating.IsThumbedUpByCurrentUser =
                    _tourReactionRepository.Exists(rating.Id, currentUserId);

                rating.IsLocalGuide =
                    _touristStatsRepository.GetByTourist(rating.UserId).IsLocalGuide;

                rating.Username = 
                    _userRepository.Get(rating.UserId).Username;
            }
        }

    }
}