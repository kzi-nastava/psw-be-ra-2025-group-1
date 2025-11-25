using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class PersonEquipmentService : IPersonEquipmentService
{
    private readonly IPersonEquipmentRepository _personEquipmentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    /*
    public PagedResult<PersonEquipmentDto> GetPaged(int page, int pageSize)
    {
        var result = _personEquipmentRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<PersonEquipmentDto>).ToList();
        return new PagedResult<PersonEquipmentDto>(items, result.TotalCount);
    }
    */

    public PersonEquipmentService(IPersonEquipmentRepository personEquipmentRepository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _personEquipmentRepository = personEquipmentRepository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    //uradjeno, gleda spisak postojece opreme
    public PagedResult<EquipmentDto> GetAvailableEquipment(int page, int pageSize)
    {
        var result = _equipmentRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<EquipmentDto>).ToList();
        return new PagedResult<EquipmentDto>(items, result.TotalCount);
    }

    public PagedResult<PersonEquipmentDto> GetPersonEquipment(long personId, int page, int pageSize)
    {
        var result = _personEquipmentRepository.GetByPersonId(personId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<PersonEquipmentDto>).ToList();
        return new PagedResult<PersonEquipmentDto>(items, result.TotalCount);
    }

    public PersonEquipmentDto AddEquipmentToPerson(PersonEquipmentDto personEquipment) //dodavanje opreme osobi
    {
        var existingPersonEquipment = _personEquipmentRepository.GetByPersonAndEquipment(personEquipment.PersonId, personEquipment.EquipmentId);

        if (existingPersonEquipment != null)
        {
            // Person already has this equipment, return the existing record
            return _mapper.Map<PersonEquipmentDto>(existingPersonEquipment);
        }
        else
        {
            // Create new PersonEquipment record
            var newPersonEquipment = new PersonEquipment(personEquipment.PersonId, personEquipment.EquipmentId);
            var result = _personEquipmentRepository.Add(newPersonEquipment);
            return _mapper.Map<PersonEquipmentDto>(result);
        }
    }

    public void RemoveEquipmentFromPerson(long personId, long equipmentId) //uklanjanje opreme od osobe
    {
        _personEquipmentRepository.Remove(personId, equipmentId);
    }
}
