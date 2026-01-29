namespace Explorer.Tours.API.Dtos
{
    public class EncounterDataDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Xp { get; set; }
        public string Type { get; set; } = "Misc";
    }
}
