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
    public long CreatorId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Difficulty { get; private set; }
    public string[] Tags { get; private set; }
    public TourStatus Status { get; private set; }
    public double Price { get; private set; }
    public List<Equipment> Equipment { get; private set; } = new List<Equipment>();

    public Tour()
    {
        Title = "";
        Description = "";
        Tags = [];
        Status = TourStatus.Draft;
    }
    public Tour(long creatorId, string title, string description, int difficulty, string[] tags, TourStatus status = TourStatus.Draft, double price = 0)
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

    public void Update(long creatorId, string title, string description, int difficulty, string[] tags, TourStatus status, double price)
    {
        CreatorId = creatorId;
        Title = title;
        Description = description;
        Difficulty = difficulty;
        Tags = tags;
        Status = status;
        Price = price;
    }

    public void AddEquipment(Equipment equipment)
    {
        Equipment.Add(equipment);
    }

    public void RemoveEquipment(Equipment equipment)
    {
        Equipment.Remove(equipment);
    }
}
