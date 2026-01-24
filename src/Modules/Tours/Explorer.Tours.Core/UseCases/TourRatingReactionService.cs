using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Explorer.Tours.Core.UseCases
{
    public class TourRatingReactionService : ITourRatingReactionService
    {
        private readonly ITourRatingRepository _tourRatingRepository;
        private readonly ITourRatingReactionRepository _tourRatingReactionRepository;
        private readonly IMapper _mapper;

        public TourRatingReactionService(ITourRatingReactionRepository repository, IMapper mapper, ITourRatingRepository tourRatingRepository)
        {
            _tourRatingReactionRepository = repository;
            _mapper = mapper;
            _tourRatingRepository = tourRatingRepository;
        }

        public PagedResult<TourRatingReactionDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRatingReactionRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingReactionDto>).ToList();
            return new PagedResult<TourRatingReactionDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingReactionDto> GetPagedByTourRating(long tourRatingId, int page, int pageSize)
        {
            var result = _tourRatingReactionRepository.GetPagedByTourRating(tourRatingId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingReactionDto>).ToList();
            return new PagedResult<TourRatingReactionDto>(items, result.TotalCount);
        }

        public TourRatingReactionDto Create(TourRatingReactionDto entity)
        {
            var result = _tourRatingReactionRepository.Create(_mapper.Map<TourRatingReaction>(entity));
            return _mapper.Map<TourRatingReactionDto>(result);
        }

        public TourRatingReactionDto Update(TourRatingReactionDto entity)
        {
            var result = _tourRatingReactionRepository.Update(_mapper.Map<TourRatingReaction>(entity));
            return _mapper.Map<TourRatingReactionDto>(result);
        }

        public void Delete(long id)
        {
            _tourRatingReactionRepository.Delete(id);
        }

        public TourRatingDto AddReaction(long tourRatingId, long userId)
        {
            // Check if the user is logged in
            if (userId == 0) throw new UnauthorizedAccessException("User must be logged in to react to a rating.");

            // Check if rating exists
            var tourRating = _tourRatingRepository.Get(tourRatingId);

            // Check if the reaction is already added
            var reaction = _tourRatingReactionRepository
                    .GetPagedByTourRating(tourRatingId, 1, int.MaxValue)
                    .Results
                    .FirstOrDefault(r => r.UserId == userId);
            if (reaction != null) throw new InvalidOperationException("User has already reacted to this rating.");

            // Check if trying to react to own post
            if (tourRating.UserId == userId) throw new InvalidOperationException("User cannot react to own rating.");

            // Create the reaction
            reaction = new TourRatingReaction(tourRatingId, userId);
            Create(_mapper.Map<TourRatingReactionDto>(reaction));
            // Increment the reactionCount on the TourRatin
            tourRating.IncrementThumbsUp();
            tourRating = _tourRatingRepository.Update(tourRating);

            return _mapper.Map<TourRatingDto>(tourRating);
        }

        public TourRatingDto RemoveReaction(long tourRatingId, long userId)
        {
            // Check if the user is logged in
            if (userId == 0) throw new UnauthorizedAccessException("User must be logged in to react to a rating.");

            var reaction = _tourRatingReactionRepository
                    .GetPagedByTourRating(tourRatingId, 1, int.MaxValue)
                    .Results
                    .FirstOrDefault(r => r.UserId == userId);

            // Check if reaction exists
            if (reaction == null) throw new NotFoundException("Could not found reaction.");

            // Delete the existing reaction
            Delete(reaction.Id);

            // Decrement the reactionCount on the TourRating
            var tourRating = _tourRatingRepository.Get(tourRatingId);
            tourRating.DecrementThumbsUp();
            tourRating = _tourRatingRepository.Update(tourRating);

            return _mapper.Map<TourRatingDto>(tourRating);
        }

        public bool HasUserReacted(long tourRatingId, long userId)
        {
            if (userId == 0) return false;

            return _tourRatingReactionRepository.Exists(tourRatingId, userId);
        }


    }
}