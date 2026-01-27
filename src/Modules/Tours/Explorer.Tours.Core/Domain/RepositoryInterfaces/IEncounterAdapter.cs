using Explorer.Encounters.API.Public;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterAdapter
    {
        Tours.API.Dtos.EncounterDto Create(Tours.API.Dtos.EncounterCreateDto encounterDto);
        void Publish(long encounterId);
        void SetKeypointId(long encounterId, long keypointId);
    }
}


