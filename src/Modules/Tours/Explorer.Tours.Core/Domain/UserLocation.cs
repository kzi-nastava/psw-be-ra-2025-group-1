namespace Explorer.Tours.Core.Domain
{
    public class UserLocation
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

        public UserLocation() { }
    }
}