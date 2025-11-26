using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long Id { get; set; } = -1;
    public long CreatorId { get; init; }= -1;
    public string Title { get; init; } = "";
    public string Description { get; init; }= "";
    public int Difficulty { get; init; } = -1;
    public string[] Tags { get; init; }= Array.Empty<string>();
    public TourStatusDTO Status { get; init; }= TourStatusDTO.Draft;
    public double Price { get; init; } = -1;

}
public enum TourStatusDTO
{
    Draft,
    Published,
    Archived
}
