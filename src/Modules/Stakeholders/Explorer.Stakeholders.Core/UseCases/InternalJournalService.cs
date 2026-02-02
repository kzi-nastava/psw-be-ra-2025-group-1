using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class InternalJournalService : IInternalJournalService
{
    private readonly IJournalRepository _journalRepo;

    public InternalJournalService(IJournalRepository journalRepo)
    {
        _journalRepo = journalRepo;
    }

    public JournalBlogLinkDto? GetByPublishedBlogId(long blogId)
    {
        var journal = _journalRepo.GetByPublishedBlogId(blogId);
        if (journal == null) return null;

        return new JournalBlogLinkDto
        {
            JournalId = journal.Id,
            BlogId = blogId,
            Collaborators = journal.Collaborators.Select(c => new JournalCollaboratorInfoDto
            {
                UserId = c.UserId,
                Username = c.User?.Username ?? ""
            }).ToList()
        };
    }
}
