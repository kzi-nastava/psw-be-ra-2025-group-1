using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Archived
}
public class Tour : Entity
{
    long CreatorId { get; init; }
    string Title { get; init; }
    string Description { get; init; }
    int Difficulty { get; init; }
    string[] Tags { get; init; }
    TourStatus Status { get; init; }
    double Price { get; init; }

    public Tour(long creatorId,string title, string description, int difficulty, string[] tags, TourStatus status=TourStatus.Draft, double price=0)
    {
        if (difficulty < 1 || difficulty > 10) throw new ArgumentException("Difficulty must be between 1 and 10.");
        if (price < 0) throw new ArgumentException("Price cannot be negative.");
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Tags = tags;
        Status = status;
        Price = price;
        CreatorId = creatorId;
    }

}
