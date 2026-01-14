using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.UseCases
{
    public class TourInfoDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
    }
}
