using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourBrowsingService : ITourBrowsingService
{
    private readonly ITourRepository _repository;

    public TourBrowsingService(ITourRepository repository)
    {
        _repository = repository;
    }

    private static TourDto ToDto(Tour t)
    {
        return new TourDto
        {
            Id = t.Id,
            CreatorId = t.CreatorId,
            Title = t.Title,
            Description = t.Description,
            Difficulty = t.Difficulty,
            Tags = t.Tags,
            Price = t.Price,
            Status = t.Status switch
            {
                TourStatus.Published => TourStatusDTO.Published,
                TourStatus.Archived => TourStatusDTO.Archived,
                _ => TourStatusDTO.Draft
            }
        };
    }

    public PagedResult<TourDto> GetPublished(int page, int pageSize)
    {
        var result = _repository.GetPublished(page, pageSize);

        // result je PagedResult<Tour>
        // radimo mapiranje SAMO nad listom:
        var mapped = result.Results
            .Select(ToDto)
            .ToList();

        return new PagedResult<TourDto>(mapped, result.TotalCount);
    }

    public TourDto? GetPublishedById(long id)
    {
        var entity = _repository.GetPublishedById(id);
        return entity == null ? null : ToDto(entity);
    }
}
