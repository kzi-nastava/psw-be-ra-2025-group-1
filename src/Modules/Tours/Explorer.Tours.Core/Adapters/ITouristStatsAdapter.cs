using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Adapters
{
    public interface ITouristStatsAdapter
    {
        void AddThumbsUp(long touristId);
        void RemoveThumbsUp(long touristId);
        bool IsLocalGuide(long touristId);

        void AddRating(long touristId);
        void RemoveRating(long touristId);
    }
}
