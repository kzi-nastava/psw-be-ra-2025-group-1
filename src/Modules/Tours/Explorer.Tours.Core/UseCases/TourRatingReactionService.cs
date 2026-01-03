using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;

namespace Explorer.Tours.Core.UseCases
{
    public class TourRatingReactionService : ITourRatingReactionService
    {
        private readonly ITourRatingReactionRepository _tourRatingReactionRepository;
        private readonly IMapper _mapper;

        public TourRatingReactionService(ITourRatingReactionRepository repository, IMapper mapper)
        {
            _tourRatingReactionRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<TourRatingReactionDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRatingReactionRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingReactionDto>).ToList();
            return new PagedResult<TourRatingReactionDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingReactionDto> GetPagedByTourRating(int tourRatingId, int page, int pageSize)
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
    }
}