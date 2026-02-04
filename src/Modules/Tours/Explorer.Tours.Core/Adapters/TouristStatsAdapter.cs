using Explorer.Encounters.API.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Adapters
{
    public class TouristStatsAdapter : ITouristStatsAdapter
    {
        private readonly ITouristStatsService _service;

        public TouristStatsAdapter(ITouristStatsService service)
        {
            _service = service;
        }

        public void AddThumbsUp(long touristId)
            => _service.AddThumbsUp(touristId);

        public void RemoveThumbsUp(long touristId)
            => _service.RemoveThumbsUp(touristId);

        public bool IsLocalGuide(long touristId)
            => _service.IsLocalGuide(touristId);

        public void AddRating(long touristId)
            => _service.AddRating(touristId);
    
        public void RemoveRating(long touristId)
            => _service.RemoveRating(touristId);
    }
}
