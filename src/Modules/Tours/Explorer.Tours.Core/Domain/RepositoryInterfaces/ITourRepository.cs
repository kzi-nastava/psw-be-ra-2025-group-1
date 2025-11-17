using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    Tour Get(long id);
    void Delete(long id);
    PagedResult<Tour> GetPaged(int page, int pageSize);
    PagedResult<Tour> GetByCreatorId(long creatorId, int page, int pageSize);
}
