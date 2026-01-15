using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.Domain.Sales;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public SaleDto Create(CreateSaleDto saleDto, long authorId)
    {
        var sale = new Sale(
            saleDto.Name,
            saleDto.DiscountPercentage,
            saleDto.StartDate,
            saleDto.EndDate,
            authorId
        );

        // Jednostavno dodaj TourIds bez provere - autor je odgovoran za validne IDs
        foreach (var tourId in saleDto.TourIds)
        {
            sale.AddTourId(tourId, authorId);
        }

        var createdSale = _saleRepository.Create(sale);
        return MapToDto(createdSale);
    }

    public SaleDto Update(long id, UpdateSaleDto saleDto, long authorId)
    {
        var sale = _saleRepository.Get(id);
        if (sale == null)
            throw new ArgumentException("Sale not found.");

        if (sale.AuthorId != authorId)
            throw new InvalidOperationException("You can only update your own sales.");

        sale.Update(
            saleDto.Name,
            saleDto.DiscountPercentage,
            saleDto.StartDate,
            saleDto.EndDate
        );

        sale.ClearTours();
        foreach (var tourId in saleDto.TourIds)
        {
            sale.AddTourId(tourId, authorId);
        }

        var updatedSale = _saleRepository.Update(sale);
        return MapToDto(updatedSale);
    }

    public void Delete(long id, long authorId)
    {
        var sale = _saleRepository.Get(id);
        if (sale == null)
            throw new ArgumentException("Sale not found.");

        if (sale.AuthorId != authorId)
            throw new InvalidOperationException("You can only delete your own sales.");

        _saleRepository.Delete(sale);
    }

    public SaleDto Get(long id)
    {
        var sale = _saleRepository.Get(id);
        if (sale == null)
            throw new ArgumentException("Sale not found.");

        return MapToDto(sale);
    }

    public PagedResult<SaleDto> GetByAuthor(long authorId, int page, int pageSize)
    {
        var result = _saleRepository.GetByAuthor(authorId, page, pageSize);
        var dtos = result.Results.Select(MapToDto).ToList();
        return new PagedResult<SaleDto>(dtos, result.TotalCount);
    }

    private SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            Name = sale.Name,
            DiscountPercentage = sale.DiscountPercentage,
            StartDate = sale.StartDate,
            EndDate = sale.EndDate,
            AuthorId = sale.AuthorId,
            TourIds = sale.TourIds,
            IsActive = sale.IsActive()
        };
    }
}