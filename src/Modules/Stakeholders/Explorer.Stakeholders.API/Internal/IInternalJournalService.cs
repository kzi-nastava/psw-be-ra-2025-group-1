using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalJournalService
{
    JournalBlogLinkDto? GetByPublishedBlogId(long blogId);
}
