namespace Explorer.Payments.API.Dtos;

public class TourPurchaseTokenDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long UserId { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public string Status { get; set; }
    public bool IsValid { get; set; }
}
