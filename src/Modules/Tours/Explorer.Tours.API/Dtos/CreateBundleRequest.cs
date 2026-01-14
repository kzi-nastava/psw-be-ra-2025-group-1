using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateBundleRequest
    {
        public string Name { get; set; } = "";
        public List<long> TourIds { get; set; } = new List<long>();
        public double Price { get; set; } = 0;
    }
}
