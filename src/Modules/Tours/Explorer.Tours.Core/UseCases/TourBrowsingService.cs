using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Services;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Dtos.Enums;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourBrowsingService : ITourBrowsingService
{
    private readonly ITourRepository _repository;
    private readonly ISaleService _saleService;

    public TourBrowsingService(ITourRepository repository, ISaleService saleService)
    {
        _repository = repository;
        _saleService = saleService;
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
                TourStatus.Published => TourStatusDto.Published,
                TourStatus.Archived => TourStatusDto.Archived,
                _ => TourStatusDto.Draft
            }
        };
    }

    private TourDto ApplySaleDiscount(TourDto tour)
    {
        var activeSales = _saleService.GetActiveSalesForTour(tour.Id);
        
        if (!activeSales.Any())
            return tour;

        // Uzmi sale sa najvećim popustom
        var bestSale = activeSales.OrderByDescending(s => s.DiscountPercentage).First();
        
        tour.IsOnSale = true;
        tour.OriginalPrice = tour.Price; // Čuvamo originalnu cenu
        tour.SaleDiscountPercentage = bestSale.DiscountPercentage;
        tour.DiscountedPrice = tour.Price * (1 - bestSale.DiscountPercentage / 100.0);
        tour.SaleId = bestSale.Id;
        tour.SaleName = bestSale.Name;
        
        // KLJUČNO: Setujemo Price na sniženu cenu jer frontend koristi Price property!
        tour.Price = tour.DiscountedPrice.Value;
        
        return tour;
    }

    public PagedResult<TourDto> GetPublished(int page, int pageSize, string? searchTerm = null, string? sortBy = null)
    {
        var result = _repository.GetPublished(0, 0);
        var tours = result.Results.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermLower = searchTerm.ToLower();
            tours = tours.Where(t => 
                t.Title.ToLower().Contains(searchTermLower) || 
                t.Description.ToLower().Contains(searchTermLower));
        }

        tours = sortBy?.ToLower() switch
        {
            "tags" or "tags-asc" => tours.OrderBy(t => t.Tags.Length > 0 ? string.Join(",", t.Tags.OrderBy(tag => tag)) : string.Empty),
            "difficulty-asc" => tours.OrderBy(t => t.Difficulty),
            "difficulty-desc" => tours.OrderByDescending(t => t.Difficulty),
            _ => tours
        };

        var tourList = tours.ToList();

        var pagedTours = tourList;
        if (pageSize > 0 && page > 0)
        {
            pagedTours = tourList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        var mapped = pagedTours
            .Select(ToDto)
            .Select(ApplySaleDiscount)
            .ToList();

        return new PagedResult<TourDto>(mapped, tourList.Count);
    }

    public TourDto? GetPublishedById(long id)
    {
        var entity = _repository.GetPublishedById(id);
        if (entity == null) return null;
        
        var dto = ToDto(entity);
        return ApplySaleDiscount(dto);
    }

    public PagedResult<TourDto> GetToursOnSale(int page, int pageSize, string? sortBy = null)
    {
        var allPublished = _repository.GetPublished(page, pageSize);
        var activeSales = _saleService.GetActiveSales();
        
        // Uzmi sve TourIds koji su na sale
        var tourIdsOnSale = activeSales
            .SelectMany(s => s.TourIds)
            .Distinct()
            .ToHashSet();

        var toursOnSale = allPublished.Results
            .Where(t => tourIdsOnSale.Contains(t.Id))
            .Select(ToDto)
            .Select(ApplySaleDiscount)
            .ToList();

        // Sortiranje
        if (sortBy == "discount-desc")
        {
            toursOnSale = toursOnSale
                .OrderByDescending(t => t.SaleDiscountPercentage ?? 0)
                .ToList();
        }
        else if (sortBy == "discount-asc")
        {
            toursOnSale = toursOnSale
                .OrderBy(t => t.SaleDiscountPercentage ?? 0)
                .ToList();
        }

        return new PagedResult<TourDto>(toursOnSale, toursOnSale.Count);
    }
}
