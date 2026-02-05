using Explorer.Tours.API.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long Id { get; set; }
    public long CreatorId { get; init; }
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public int Difficulty { get; init; } = -1;
    public string[] Tags { get; init; }= Array.Empty<string>();
    public TourStatusDto Status { get; init; }= TourStatusDto.Draft;
    public double Price { get; set; } = -1;  // Changed from init to set so we can apply discounts
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime PublishedAt { get; init; }
    public DateTime ArchivedAt { get; init; }
    public List<TransportTimeDto> TransportTimes { get; init; } = [];
    public List<KeypointDto> Keypoints { get; init; }
    public List<EquipmentDto> Equipment { get; init; }
    public MapMarkerDto? MapMarker { get; init; }
    
    // Sale/Discount fields
    public double? OriginalPrice { get; set; }
    public double? DiscountedPrice { get; set; }
    public bool IsOnSale { get; set; }
    public int? SaleDiscountPercentage { get; set; }
    public long? SaleId { get; set; }
    public string? SaleName { get; set; }
}

