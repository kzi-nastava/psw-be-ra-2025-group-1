using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
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

    public PersonEquipmentService(IPersonEquipmentRepository personEquipmentRepository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _personEquipmentRepository = personEquipmentRepository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

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

    public PersonEquipmentDto AddEquipmentToPerson(long personId, long equipmentId, int quantity)
    {
        var existingPersonEquipment = _personEquipmentRepository.GetByPersonAndEquipment(personId, equipmentId);

        if (existingPersonEquipment != null)
        {
            // Update existing quantity
            // Instead of creating a new PersonEquipment and trying to set Id (which is protected),
            // update the quantity of the existing entity and save it.
            var updatedPersonEquipment = new PersonEquipment(existingPersonEquipment.PersonId, existingPersonEquipment.EquipmentId, existingPersonEquipment.Quantity + quantity);
            typeof(Entity).GetProperty("Id")?.SetValue(updatedPersonEquipment, existingPersonEquipment.Id); // Reflection workaround if needed

            var updatedResult = _personEquipmentRepository.Add(updatedPersonEquipment);
            return _mapper.Map<PersonEquipmentDto>(updatedResult);
        }
        else
        {
            // Create new PersonEquipment
            var personEquipment = new PersonEquipment(personId, equipmentId, quantity);
            var result = _personEquipmentRepository.Add(personEquipment);
            return _mapper.Map<PersonEquipmentDto>(result);
        }
    }

    public void RemoveEquipmentFromPerson(long personId, long equipmentId)
    {
        _personEquipmentRepository.Remove(personId, equipmentId);
    }
}
