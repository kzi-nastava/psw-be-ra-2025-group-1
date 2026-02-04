using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TouristMapMarkerDto
    {
        public long TouristId { get; init; }
        public long MapMarkerId { get; init; }
        public bool IsActive { get; init; }
    }
}
