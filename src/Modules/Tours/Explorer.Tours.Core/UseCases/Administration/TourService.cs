using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration;

public class TourService : ITourService
{
    public TourDto Create(TourDto tourdto)
    {
        Tour tour = new Tour
        {
            CreatorId = -1, // TODO Get from context real creator Id
            Title = tourdto.Title,
            Description = tourdto.Description,
        };
        return tourdto;
    }

    public void Delete(long id)
    {
        throw new NotImplementedException();
    }

    public List<TourDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public PagedResult<TourDto> GetByCreator(long creatorId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public TourDto GetById(long id)
    {
        throw new NotImplementedException();
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public TourDto Update(TourDto problem)
    {
        throw new NotImplementedException();
    }
}
