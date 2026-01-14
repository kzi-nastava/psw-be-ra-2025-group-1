using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.Domain
{
    public class TourBundle : AggregateRoot
    {
        public long CreatorId { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }  
        public List<Tour> Tours { get; private set; }  
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public TourBundle()
        {
            Name = "";
            Price = 0;
            Tours = new List<Tour>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public TourBundle(long creatorId, string name, List<Tour> tours, double price = 0)
        {
            CreatorId = creatorId;
            Name = name;
            Tours = tours ?? new List<Tour>();
            Price = price;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTour(Tour tour)
        {
            if (Tours.Any(t => t.Id == tour.Id))
                throw new InvalidOperationException("Tour is already in the bundle");

            Tours.Add(tour);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveTour(long tourId)
        {
            var tour = Tours.FirstOrDefault(t => t.Id == tourId)
                ?? throw new InvalidOperationException("Tour not found in bundle");

            Tours.Remove(tour);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, double price)
        {
            Name = name;
            Price = price;
            UpdatedAt = DateTime.UtcNow;
        }

        public double GetTotalToursPrice()
        {
            return Tours.Sum(t => t.Price);
        }
    }
}
