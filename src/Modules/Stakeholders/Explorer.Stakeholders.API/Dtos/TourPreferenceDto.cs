using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TourPreferenceDto
    {
        public long Id { get; set; }
        public long PersonId { get; set; }

        /// <summary>
        /// Null if no preference
        /// </summary>
        public double? Difficulty { get; init; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? WalkRating { get; init; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? BicycleRating { get; init; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? CarRating { get; init; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? BoatRating { get; init; }

        /// <summary>
        /// Empty / null if no preference
        /// </summary>
        public List<string>? PreferedTags { get; set; }

    }
}
