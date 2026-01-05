using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourRatingDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TourExecutionId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public double CompletedProcentage { get; set; }
        public int ThumbsUpCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
