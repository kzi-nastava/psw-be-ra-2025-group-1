using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class MapMarker : Entity
    {
        public string ImageUrl { get; set; }
        public bool IsStandalone { get; private set; } // false if marker is collected through a tour/encounter/etc, true if predefined

        public MapMarker(string ImageUrl)
        {
            this.ImageUrl = ImageUrl;
            IsStandalone = false;
        }

        public MapMarker Update(MapMarker updatedMarker)
        {
            this.ImageUrl = updatedMarker.ImageUrl;

            return this;
        }

        public bool SetAsStandalone()
        {
            IsStandalone = true;
            return IsStandalone;
        }
    }
}
