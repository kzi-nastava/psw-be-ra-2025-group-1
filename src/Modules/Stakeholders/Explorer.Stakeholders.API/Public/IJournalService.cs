using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IJournalService
{
    JournalDto Create(long userId, JournalCreateDto dto);
    List<JournalDto> GetMine(long userId);
    JournalDto Update(long userId, long journalId, JournalUpdateDto dto);
    void Delete(long userId, long journalId);
    JournalDto Publish(long userId, long journalId);

}
