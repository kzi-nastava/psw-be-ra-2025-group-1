using AutoMapper;
using Explorer.Blog.API.Internal;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class JournalService : IJournalService
{
    private readonly IJournalRepository _repo;
    private readonly IMapper _mapper; 
    private readonly IInternalBlogService _internalBlogService;

    public JournalService(IJournalRepository repo, IMapper mapper, IInternalBlogService internalBlogService)
    {
        _repo = repo;
        _mapper = mapper;
        _internalBlogService = internalBlogService;
    }

    public JournalDto Create(long userId, JournalCreateDto dto)
    {
            var journal = new Journal(dto.Content, userId, dto.Title, dto.Latitude, dto.Longitude, dto.LocationName);

            journal.SetMedia(dto.Images, dto.Videos);
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

    public JournalDto Publish(long userId, long journalId)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (journal.UserId != userId)
            throw new UnauthorizedAccessException("Nemate dozvolu.");

        if (journal.PublishedBlogId != null)
            throw new InvalidOperationException("Dnevnik je već objavljen.");

        // 1) napravi Blog (minimalno: title + content)
        var blogCreated = _internalBlogService.CreateFromJournal(
            userId,
            journal.Title,
            journal.Content
            );

        // 2) markiraj journal kao published i upisi blogId
        journal.Publish(blogCreated.Id);

        _repo.Update(journal);

        return _mapper.Map<JournalDto>(journal);
    }

}