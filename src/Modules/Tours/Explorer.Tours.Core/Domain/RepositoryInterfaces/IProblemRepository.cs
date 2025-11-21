using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IProblemRepository
{
    Problem Create(Problem problem);
    Problem Update(Problem problem);
    Problem Get(long id);
    void Delete(long id);
    PagedResult<Problem> GetPaged(int page, int pageSize);
    PagedResult<Problem> GetByCreatorId(long creatorId, int page, int pageSize);
}
