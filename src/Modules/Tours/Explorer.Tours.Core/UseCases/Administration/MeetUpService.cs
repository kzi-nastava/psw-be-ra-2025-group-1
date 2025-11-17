using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class MeetUpService : IMeetUpService
    {
        private readonly IMeetUpRepository _meetUpRepository;
        private readonly IMapper _mapper;
        public MeetUpService(IMeetUpRepository repository, IMapper mapper)
        {
            _meetUpRepository = repository;
            _mapper = mapper;
        }
        public PagedResult<MeetUpDto> GetPaged(int page, int pageSize)
        {
            var result = _meetUpRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<MeetUpDto>).ToList();
            return new PagedResult<MeetUpDto>(items, result.TotalCount);
        }
        public PagedResult<MeetUpDto> GetPagedByUser(long userId, int page, int pageSize)
        {
            var result = _meetUpRepository.GetPagedByUser(userId, page, pageSize);
            var items = result.Results.Select(_mapper.Map<MeetUpDto>).ToList();
            return new PagedResult<MeetUpDto>(items, result.TotalCount);
        }
        public MeetUpDto Create(MeetUpDto entity)
        {
            var result = _meetUpRepository.Create(_mapper.Map<MeetUp>(entity));
            return _mapper.Map<MeetUpDto>(result);
        }
        public MeetUpDto Update(MeetUpDto entity)
        {
            var result = _meetUpRepository.Update(_mapper.Map<MeetUp>(entity));
            return _mapper.Map<MeetUpDto>(result);
        }
        public void Delete(long id)
        {
            _meetUpRepository.Delete(id);
        }
    }
}
