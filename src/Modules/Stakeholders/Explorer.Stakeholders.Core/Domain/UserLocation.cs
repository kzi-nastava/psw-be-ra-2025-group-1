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
        public long UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

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
