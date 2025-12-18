using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class KeypointProgressDto
    {
        public long Id { get; set; }
        public DateTime ReachedAt { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
