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
        public string? Name { get; set; }
        public string ImagePathUrl { get; set; }

        public MapMarker(string imagePathUrl, string name = null)
        {
            this.Name = name;
            this.ImagePathUrl = imagePathUrl;
        }

        public MapMarker Update(MapMarker updatedMarker)
        {
            this.Name = updatedMarker.Name;
            this.ImagePathUrl = updatedMarker.ImagePathUrl;

            return this;
        }
    }
}
