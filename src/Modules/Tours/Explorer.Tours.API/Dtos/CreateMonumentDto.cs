namespace Explorer.Tours.API.Dtos
{
    public class CreateMonumentDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
