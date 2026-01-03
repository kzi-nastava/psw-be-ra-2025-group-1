using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourRatingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TourExecutionId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public int ThumbsUpCount { get; set; }
    }
}
