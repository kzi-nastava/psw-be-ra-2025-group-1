using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public;

public interface IEncounterService
{
    List<EncounterDto> GetActiveEncounters();
    EncounterDto GetById(long id);
    EncounterDto Create(EncounterCreateDto dto);
    EncounterDto Update(long id, EncounterCreateDto dto);
    void Publish(long id);
    void Archive(long id);
    void Delete(long id);
}