using AutoMapper;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class MonumentService : IMonumentService
    {
        private readonly IMonumentRepository _monumentRepository;
        private readonly IMapper _mapper;

        public MonumentService(IMonumentRepository monumentRepository, IMapper mapper)
        {
            _monumentRepository = monumentRepository;
            _mapper = mapper;
        }

        public PagedResult<MonumentDto> GetPaged(int page, int pageSize)
        {
            var result = _monumentRepository.GetPaged(page, pageSize);
            var items = result.Results.Select(_mapper.Map<MonumentDto>).ToList();
            return new PagedResult<MonumentDto>(items, result.TotalCount);
        }

        public List<MonumentDto> GetAll()
        {
            var monuments = _monumentRepository.GetAll();
            return monuments.Select(_mapper.Map<MonumentDto>).ToList();
        }

        public MonumentDto GetById(long id)
        {
            var monument = _monumentRepository.Get(id);
            return _mapper.Map<MonumentDto>(monument);
        }

        public MonumentDto Create(CreateMonumentDto monumentDto)
        {
            var monument = new Monument(monumentDto.Name, monumentDto.Description, monumentDto.Longitude, monumentDto.Latitude);
            var createdMonument = _monumentRepository.Create(monument);
            return _mapper.Map<MonumentDto>(createdMonument);
        }

        public MonumentDto Update(MonumentDto monumentDto)
        {
            var monument = _mapper.Map<Monument>(monumentDto);
            var updatedMonument = _monumentRepository.Update(monument);
            return _mapper.Map<MonumentDto>(updatedMonument);
        }

        public void Delete(long id)
        {
            _monumentRepository.Delete(id);
        }
    }
}
