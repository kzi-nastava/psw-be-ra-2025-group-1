using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TourPreference
    {
        public int TouristId { get; init; }
        public User? User { get; init; }
        public double Difficulty { get; init; }
        public int WalkRating { get; init; }
        public int BicycleRating { get; init; }
        public int CarRating { get; init; }
        public int BoatRating { get; init; }

        public List<string>? PreferedTags;
    }
}
