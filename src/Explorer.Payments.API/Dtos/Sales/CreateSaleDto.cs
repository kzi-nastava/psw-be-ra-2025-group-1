namespace Explorer.Payments.API.Dtos.Sales;

public class CreateSaleDto
{
    public string Name { get; set; } = string.Empty;
    public int DiscountPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<long> TourIds { get; set; } = new();
}