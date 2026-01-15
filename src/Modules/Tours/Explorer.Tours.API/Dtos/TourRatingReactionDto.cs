using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourRatingReactionDto
    {
        public int Id { get; set; }
        public long TourRatingId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
