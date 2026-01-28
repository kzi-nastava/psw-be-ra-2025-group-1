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
        public long UserId { get; set; }

        /// <summary>
        /// Null if no preference
        /// </summary>
        public double? Difficulty { get; set; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? WalkRating { get; set; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? BicycleRating { get; set; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? CarRating { get; set; }
        /// <summary>
        /// Values from 0 to 3, null if no preference
        /// </summary>
        public int? BoatRating { get; set; }

        /// <summary>
        /// Empty / null if no preference
        /// </summary>
        public List<string>? PreferedTags { get; set; }

    }
}
