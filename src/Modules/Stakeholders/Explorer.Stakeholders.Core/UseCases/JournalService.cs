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
    private readonly IUserRepository _userRepo;

    public JournalService(IJournalRepository repo, IMapper mapper, IInternalBlogService internalBlogService, IUserRepository userRepo)
    {
        _repo = repo;
        _mapper = mapper;
        _internalBlogService = internalBlogService;
        _userRepo = userRepo;
    }

    public JournalDto Create(long userId, JournalCreateDto dto)
    {
            var journal = new Journal(dto.Content, userId, dto.Title, dto.Latitude, dto.Longitude, dto.LocationName);

            journal.SetMedia(dto.Images, dto.Videos);
            journal = _repo.Add(journal);

            return MapForUser(journal, userId);
    }

    public List<JournalDto> GetMine(long userId)
    {
        var list = _repo.GetAccessibleByUserId(userId).ToList();

        return list.Select(j => MapForUser(j, userId)).ToList();
    }

    public JournalDto GetById(long userId, long journalId)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (!(journal.IsOwner(userId) || journal.IsCollaborator(userId)))
            throw new UnauthorizedAccessException("Nemate pristup ovom dnevniku.");

        return MapForUser(journal, userId);
    }

    public JournalDto Update(long userId, long journalId, JournalUpdateDto dto)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (!(journal.IsOwner(userId) || journal.IsCollaborator(userId)))
            throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovaj dnevnik.");

        journal.Update(dto.Content, dto.Title);

        _repo.Update(journal);

        return MapForUser(journal, userId);
    }

    public void Delete(long userId, long journalId)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (!journal.IsOwner(userId))
            throw new UnauthorizedAccessException("Nemate dozvolu da obrišete ovaj dnevnik.");

        _repo.Delete(journal);
    }

    public JournalDto Publish(long userId, long journalId)
    {
        var journal = _repo.GetById(journalId)
                      ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        if (!(journal.IsOwner(userId) || journal.IsCollaborator(userId)))
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

        return MapForUser(journal, userId);
    }


    public JournalDto AddCollaborator(long ownerId, long journalId, string query)
    {
        var journal = _repo.GetById(journalId) ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");

        // lookup user
        var user = _userRepo.FindByUsername(query);
        if (user == null) throw new ArgumentException("User does not exist.");

        journal.AddCollaborator(ownerId, user.Id);
        _repo.Update(journal);

        return MapForUser(journal, ownerId);
    }
    public JournalDto RemoveCollaborator(long ownerId, long journalId, long collaboratorUserId)
    {
        var journal = _repo.GetById(journalId) ?? throw new KeyNotFoundException("Dnevnik nije pronađen.");
        journal.RemoveCollaborator(ownerId, collaboratorUserId);
        _repo.Update(journal);
        return MapForUser(journal, ownerId);
    }

    private JournalDto MapForUser(Journal journal, long currentUserId)
    {
        var dto = _mapper.Map<JournalDto>(journal);

        dto.CanManageCollaborators = journal.IsOwner(currentUserId);
        dto.CanEdit = journal.IsOwner(currentUserId) || journal.IsCollaborator(currentUserId);
        dto.CanDelete = journal.IsOwner(currentUserId);

        // Collaborators + username 
        dto.Collaborators = journal.Collaborators
            .Select(c => new CollaboratorDto
            {
                UserId = c.UserId,
                Username = c.User?.Username ?? "" 
            })
            .ToList();

        return dto;
    }


}