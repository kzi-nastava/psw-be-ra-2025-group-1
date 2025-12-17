using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IMonumentService
    {
        PagedResult<MonumentDto> GetPaged(int page, int pageSize);
        List<MonumentDto> GetAll();
        MonumentDto GetById(long id);
        MonumentDto Create(MonumentDto meetup);
        MonumentDto Update(MonumentDto meetup);
        void Delete(long id);
    }
}
