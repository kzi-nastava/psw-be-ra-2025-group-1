using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class TourPreference : Entity
    {
        public long UserId { get; init; }

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
        public List<string>? PreferedTags;
    }
}
