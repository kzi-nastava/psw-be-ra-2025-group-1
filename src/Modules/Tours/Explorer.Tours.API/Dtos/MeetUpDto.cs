namespace Explorer.Tours.API.Dtos
{
    public class MeetUpDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public long UserId { get; set; }
    }
}
