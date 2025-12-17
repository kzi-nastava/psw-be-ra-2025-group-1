using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class JournalService : IJournalService
{
    private readonly IJournalRepository _repo;
    private readonly IMapper _mapper;

    public JournalService(IJournalRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public JournalDto Create(long userId, JournalCreateDto dto)
    {
            var journal = new Journal(dto.Content, userId, dto.Title, dto.Location);

            journal = _repo.Add(journal);

            return _mapper.Map<JournalDto>(journal);
    }

    public List<JournalDto> GetMine(long userId)
    {
        var list = _repo.GetByUserId(userId).ToList();

        return _mapper.Map<List<JournalDto>>(list);
    }

    public JournalDto Update(long userId, long journalId, JournalUpdateDto dto)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (journal.UserId != userId)
            throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovaj dnevnik.");

        journal.Update(dto.Content, dto.Title);

        _repo.Update(journal);

        return _mapper.Map<JournalDto>(journal);
    }

    public void Delete(long userId, long journalId)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (journal.UserId != userId)
            throw new UnauthorizedAccessException("Nemate dozvolu da obrišete ovaj dnevnik.");

        _repo.Delete(journal);
    }
}