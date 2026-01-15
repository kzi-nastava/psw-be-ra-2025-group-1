using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class UpdateBundleRequest
    {
        public string Name { get; set; } = "";
        public double Price { get; set; } = 0;
    }
}
