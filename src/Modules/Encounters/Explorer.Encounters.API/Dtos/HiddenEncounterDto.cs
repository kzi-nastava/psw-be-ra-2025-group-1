using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    // This Dto is to be used only when updating the encounter
    public class HiddenEncounterDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Xp { get; set; }
        public string Status { get; set; } = "";
        public string Type { get; set; } = "";
        public double? Range { get; set; }
        public string? ImagePath { get; set; } = "";
        public List<string>? Hints { get; set; } = new List<string>();
        public double? HiddenLongitude { get; set; }
        public double? HiddenLatitude { get; set; }
    }
}