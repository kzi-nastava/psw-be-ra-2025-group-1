using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases;

public class EncounterService : IEncounterService
{
    private readonly IEncounterRepository _repository;
    private readonly IMapper _mapper;

    public EncounterService(IEncounterRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public List<EncounterDto> GetActiveEncounters()
    {
        return _mapper.Map<List<EncounterDto>>(_repository.GetActive());
    }

    public EncounterDto GetById(long id)
    {
        var encounter = _repository.GetById(id);
        return _mapper.Map<EncounterDto>(encounter);
    }

    public EncounterDto Create(EncounterCreateDto dto)
    {
        var encounter = new Encounter(
            dto.Title,
            dto.Description,
            dto.Location,
            dto.Xp,
            Enum.Parse<EncounterType>(dto.Type)
        );

        var created = _repository.Create(encounter);
        return _mapper.Map<EncounterDto>(created);
    }

    public EncounterDto Update(long id, EncounterCreateDto dto)
    {
        var encounter = _repository.GetById(id);
        
        encounter.Update(dto.Title, dto.Description, dto.Location, dto.Xp, Enum.Parse<EncounterType>(dto.Type));
        var updated = _repository.Update(encounter);
        return _mapper.Map<EncounterDto>(updated);
    }

    public void Publish(long id)
    {
        var encounter = _repository.GetById(id);
        encounter.Publish();
        _repository.Update(encounter);
    }

    public void Archive(long id)
    {
        var encounter = _repository.GetById(id);
        encounter.Archive();
        _repository.Update(encounter);
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }
}
