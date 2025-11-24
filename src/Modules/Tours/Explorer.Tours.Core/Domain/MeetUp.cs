using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class MeetUp : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime Date { get; private set; }
        public int Longitude { get; private set; }
        public int Latitude { get; private set; }
        public long UserId { get; private set; }

        public MeetUp(string name, string? description, DateTime date, int longitude, int latitude, long userId)
        {
            Name = name;
            Description = description;
            Date = date;
            Longitude = longitude;
            Latitude = latitude;
            UserId = userId;
            Validate();
        }
        
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name.");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description.");
            if (Date == default) throw new ArgumentException("Invalid Date.");
            if (UserId < 0) throw new ArgumentException("Invalid UserId.");
        }
    }
}
