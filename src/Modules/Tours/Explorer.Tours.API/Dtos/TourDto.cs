using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourDto
{
    long CreatorId { get; init; }= -1;
    string Title { get; init; } = "";
    string Description { get; init; }= "";
    int Difficulty { get; init; } = -1;
    string[] Tags { get; init; }= Array.Empty<string>();
    string Status { get; init; }= "Draft"; // Maybe should convert to TourStatus enum
    double Price { get; init; } = -1;

}
