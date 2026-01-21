namespace Explorer.Payments.API.Dtos;

public class BundleDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public List<long> TourIds { get; set; }
}
