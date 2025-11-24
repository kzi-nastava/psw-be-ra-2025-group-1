using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class UserLocation: Entity
    {
        public long UserId { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime Timestamp { get; init; }

        public UserLocation(long userId, double latitude, double longitude, DateTime timestamp)
        {
            UserId = userId;
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
        }
        public UserLocation()
        {
            UserId = -1;
            Latitude = -1;
            Longitude = -1;
            Timestamp = DateTime.MinValue;
        }
    }
}
