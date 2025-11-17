using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IProblemService
{
    PagedResult<ProblemDto> GetPaged(int page, int pageSize);
    
    PagedResult<ProblemDto> GetByCreator(long creatorId, int page, int pageSize);
    ProblemDto Create(ProblemDto problem);
    ProblemDto Update(ProblemDto problem);
    void Delete(long id);
}

