using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{

    /// <summary>
    /// All types of transport available for a tour
    /// </summary>
    public enum TransportType
    {
        Unknown,
        Foot,
        Bike,
        Car,
    }

    public class TransportTime : Entity
    {
        public TransportType Type { get; private set; }

        /// <summary>
        /// Minutes required for the transport type
        /// </summary>
        public int Duration { get; private set; }

        public long TourId { get; private set; }
        public Tour Tour { get; private set; }
        public TransportTime()
        {
            Type = TransportType.Foot;
            Duration = 0;
            Tour = null!;
        }

        public TransportTime(TransportType type, int duration, Tour tour)
        {
            if (duration < 0) throw new ArgumentException("Duration cannot be negative.");
            Type = type;
            Duration = duration;
            Tour = tour;
            TourId = tour.Id;
        }

    }
}
