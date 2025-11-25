namespace Explorer.Tours.API.Dtos
{
    public class PersonEquipmentDto
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public long EquipmentId { get; set; }
        public EquipmentDto? Equipment { get; set; }
    }
}
