namespace Explorer.Payments.API.Dtos.Sales;

public class SaleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long AuthorId { get; set; }
    public List<long> TourIds { get; set; } = new();
    public bool IsActive { get; set; }
}