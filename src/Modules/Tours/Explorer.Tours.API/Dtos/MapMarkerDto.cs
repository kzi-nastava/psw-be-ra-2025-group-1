using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class MapMarkerDto
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public bool IsStandalone { get; private set; } // false if marker is collected through a tour/encounter/etc, true if predefined
    }
}
