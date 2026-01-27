//using Explorer.Tours.API.Dtos;
using Explorer.Encounters.API.Public;
using AutoMapper;
using Explorer.Encounters.API.Dtos;

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
        return _mapper.Map<EncounterDto>(_encounterService.Create(_mapper.Map<EncounterCreateDto>(encounterDto)));
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