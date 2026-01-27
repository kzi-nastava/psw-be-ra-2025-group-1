using Explorer.Tours.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using AutoMapper;

namespace Explorer.Tours.Core.Adapters;

public class EncounterAdapter : IEncounterAdapter
{
    private readonly IEncounterService _encounterService;
    private readonly IMapper _mapper;

    public EncounterAdapter(IEncounterService encounterService, IMapper mapper)
    {
        _encounterService = encounterService;
        _mapper = mapper;
    }

    public EncounterDto Create(EncounterCreateDto encounterDto)
    {
        return _mapper.Map< Explorer.Tours.API.Dtos.EncounterDto>(_encounterService.Create(_mapper.Map<Explorer.Encounters.API.Dtos.EncounterCreateDto>(encounterDto)));
    }

    public void Publish(long encounterId)
    {
        _encounterService.Publish(encounterId);
    }

    public void SetKeypointId(long encounterId, long keypointId)
    {
        _encounterService.SetKeypointId(encounterId, keypointId);
    }
}