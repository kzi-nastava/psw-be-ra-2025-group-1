using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    public long CreatorId { get; init; }= -1;
    public string Title { get; init; } = "";
    public string Description { get; init; }= "";
    public int Difficulty { get; init; } = -1;
    public string[] Tags { get; init; }= Array.Empty<string>();
    public string Status { get; init; }= "Draft"; // Maybe should convert to TourStatus enum
    public double Price { get; init; } = -1;

}
