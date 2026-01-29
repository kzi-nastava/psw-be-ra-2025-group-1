using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;

namespace Explorer.Tours.Core.Adapters
{
    public interface IEncounterAdapter
    {
        EncounterDto Create(EncounterCreateDto encounterDto);
        void Publish(long encounterId);
        void SetKeypointId(long encounterId, long keypointId);
    }
}


