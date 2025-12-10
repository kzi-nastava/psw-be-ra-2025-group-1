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

    public PersonEquipmentService(IPersonEquipmentRepository personEquipmentRepository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _personEquipmentRepository = personEquipmentRepository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public PagedResult<EquipmentDto> GetAvailableEquipment(long personId, int page, int pageSize)
    {
        // Get all equipment
        var allEquipment = _equipmentRepository.GetPaged(page, pageSize);
        
        // Get equipment IDs that the person already owns
        var ownedEquipment = _personEquipmentRepository.GetByPersonId(personId, 0, 0);
        var ownedEquipmentIds = ownedEquipment.Results.Select(pe => pe.EquipmentId).ToHashSet();
        
        // Filter out owned equipment
        var availableEquipment = allEquipment.Results
            .Where(e => !ownedEquipmentIds.Contains(e.Id))
            .ToList();
        
        var items = availableEquipment.Select(_mapper.Map<EquipmentDto>).ToList();
        
        // Return with adjusted total count
        var availableTotalCount = allEquipment.TotalCount - ownedEquipmentIds.Count;
        return new PagedResult<EquipmentDto>(items, availableTotalCount);
    }

    public PagedResult<PersonEquipmentDto> GetPersonEquipment(long personId, int page, int pageSize)
    {
        var result = _personEquipmentRepository.GetByPersonId(personId, page, pageSize);
        var items = new List<PersonEquipmentDto>();
        
        foreach (var personEquipment in result.Results)
        {
            var dto = _mapper.Map<PersonEquipmentDto>(personEquipment);
            
            // Load the equipment details
            try
            {
                var equipment = _equipmentRepository.Get(personEquipment.EquipmentId);
                dto.Equipment = _mapper.Map<EquipmentDto>(equipment);
            }
            catch (NotFoundException)
            {
                // Equipment not found, skip or handle as needed
                dto.Equipment = null;
            }
            
            items.Add(dto);
        }
        
        return new PagedResult<PersonEquipmentDto>(items, result.TotalCount);
    }

    public PersonEquipmentDto AddEquipmentToPerson(PersonEquipmentDto personEquipment)
    {
        var existingPersonEquipment = _personEquipmentRepository.GetByPersonAndEquipment(personEquipment.PersonId, personEquipment.EquipmentId);

        if (existingPersonEquipment != null)
        {
            // Person already has this equipment, return the existing record
            var existingDto = _mapper.Map<PersonEquipmentDto>(existingPersonEquipment);
            
            // Load equipment details
            try
            {
                var equipment = _equipmentRepository.Get(existingPersonEquipment.EquipmentId);
                existingDto.Equipment = _mapper.Map<EquipmentDto>(equipment);
            }
            catch (NotFoundException)
            {
                existingDto.Equipment = null;
            }
            
            return existingDto;
        }
        else
        {
            var equipment = _equipmentRepository.Get(personEquipment.EquipmentId);
            if (equipment == null)
            {
                throw new NotFoundException($"Equipment with ID {personEquipment.EquipmentId} not found");
            }
            // Create new PersonEquipment record
            var newPersonEquipment = new PersonEquipment(personEquipment.PersonId, personEquipment.EquipmentId);
            var result = _personEquipmentRepository.Add(newPersonEquipment);
            
            var resultDto = _mapper.Map<PersonEquipmentDto>(result);
            
            // Load equipment details
            equipment = _equipmentRepository.Get(result.EquipmentId);
            resultDto.Equipment = _mapper.Map<EquipmentDto>(equipment);

            return resultDto;
        }
    }

    public void RemoveEquipmentFromPerson(long personId, long equipmentId)
    {
        _personEquipmentRepository.Remove(personId, equipmentId);
    }
}
