using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public enum TransportTypeDto
    {
        Foot,
        Bike,
        Car,
    }

    public class TransportTimeDto
    {
        public long Id { get; set; }
        public TransportTypeDto Type { get; set; }
        public int Duration { get; set; }
        public long TourId { get; set; }
    }
}
