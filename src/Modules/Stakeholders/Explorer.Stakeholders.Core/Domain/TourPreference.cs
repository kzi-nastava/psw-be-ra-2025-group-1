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

    // stuff for running migrations from Package Manager Console:
    //Add-Migration -Name Init -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API
    //Update-Database -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API

}
