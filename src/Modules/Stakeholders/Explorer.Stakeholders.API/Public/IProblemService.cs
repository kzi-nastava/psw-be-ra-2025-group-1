using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IProblemService
{
    PagedResult<ProblemDto> GetPaged(int page, int pageSize);
    
    PagedResult<ProblemDto> GetByCreator(long creatorId, int page, int pageSize);
    ProblemDto Create(ProblemDto problem);
    ProblemDto Update(ProblemDto problem);
    void Delete(long id);
}
