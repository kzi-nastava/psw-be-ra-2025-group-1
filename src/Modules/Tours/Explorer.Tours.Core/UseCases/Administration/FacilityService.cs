using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using DomainFacilityCategory = Explorer.Tours.Core.Domain.FacilityCategory;
using DtoFacilityCategory = Explorer.Tours.API.Dtos.FacilityCategory;

namespace Explorer.Tours.Core.UseCases.Administration;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly IMapper _mapper;

    public FacilityService(IFacilityRepository repository, IMapper mapper)
    {
        _facilityRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<FacilityDto> GetPaged(int page, int pageSize)
    {
        var result = _facilityRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<FacilityDto>).ToList();
        return new PagedResult<FacilityDto>(items, result.TotalCount);
    }

    public List<FacilityDto> GetAll()
    {
        var facilities = _facilityRepository.GetAll();
        return facilities.Select(_mapper.Map<FacilityDto>).ToList();
    }

    public FacilityDto GetById(long id)
    {
        var facility = _facilityRepository.Get(id);
        return _mapper.Map<FacilityDto>(facility);
    }

    public List<FacilityDto> GetByCategory(DtoFacilityCategory category)
    {
        var facilities = _facilityRepository.GetByCategory((DomainFacilityCategory)category);
        return facilities.Select(_mapper.Map<FacilityDto>).ToList();
    }

    public FacilityDto Create(FacilityDto facilityDto)
    {
        if (_facilityRepository.ExistsByName(facilityDto.Name))
        {
            throw new EntityValidationException("Facility with this name already exists.");
        }

        var facility = new Facility(facilityDto.Name, facilityDto.Latitude, facilityDto.Longitude, 
            (DomainFacilityCategory)facilityDto.Category);
        var result = _facilityRepository.Create(facility);
        return _mapper.Map<FacilityDto>(result);
    }

    public FacilityDto Update(FacilityDto facilityDto)
    {
        var facility = _facilityRepository.Get(facilityDto.Id);
        facility.Update(facilityDto.Name, facilityDto.Latitude, facilityDto.Longitude, 
            (DomainFacilityCategory)facilityDto.Category);
        
        var result = _facilityRepository.Update(facility);
        return _mapper.Map<FacilityDto>(result);
    }

    public void Delete(long id)
    {
        _facilityRepository.Delete(id);
    }
}
