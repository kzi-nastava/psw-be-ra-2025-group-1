using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
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
        private readonly IMapper _mapper;

        public TourRatingService(ITourRatingRepository repository, IMapper mapper)
        {
            _tourRatingRepository = repository;
            _mapper = mapper;
        }

        public PagedResult<TourRatingDto> GetPaged(int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByUser(int userId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPagedByUser(userId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public PagedResult<TourRatingDto> GetPagedByTourExecution(int tourExecutionId, int page, int pageSize)
        {
            var result = _tourRatingRepository.GetPagedByTourExecution(tourExecutionId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<TourRatingDto>).ToList();
            return new PagedResult<TourRatingDto>(items, result.TotalCount);
        }

        public TourRatingDto Create(TourRatingDto entity)
        {
            var result = _tourRatingRepository.Create(_mapper.Map<TourRating>(entity));
            return _mapper.Map<TourRatingDto>(result);
        }

        public TourRatingDto Update(TourRatingDto entity)
        {
            var result = _tourRatingRepository.Update(_mapper.Map<TourRating>(entity));
            return _mapper.Map<TourRatingDto>(result);
        }

        public void Delete(long id)
        {
            _tourRatingRepository.Delete(id);
        }
    }
}