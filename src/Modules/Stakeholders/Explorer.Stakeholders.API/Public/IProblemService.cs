using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IProblemService
{
    PagedResult<ProblemDto> GetPaged(int page, int pageSize);
    PagedResult<ProblemDto> GetByCreator(long creatorId, int page, int pageSize);
    PagedResult<ProblemDto> GetByAuthor(long authorId, int page, int pageSize);
    ProblemDto Get(long id, long userId);
    ProblemDto GetByIdForAdmin(long id);
    ProblemDto Create(ProblemDto problem);
    ProblemDto Update(ProblemDto problem);
    void Delete(long id);
    
    // nove metode za tour issue lifecycle
    ProblemDto ChangeProblemStatus(long problemId, long touristId, ProblemStatus newStatus, string? comment);
    ProblemDto SetAdminDeadline(long problemId, DateTime deadline);
    List<ProblemDto> GetUnresolvedOlderThan(int days = 5);
    
    // Admin akcije
    ProblemDto CloseProblemAsAdmin(long problemId, long adminId, string? comment = null);
    void PenalizeAuthor(long problemId, long adminId);
    
    // Notifikacije za istekle rokove
    void CheckAndNotifyExpiredDeadlines(long adminId);
}
