using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    // Connects One-Many User-MapMarker
    // represents tourist's map marker collection
    public class TouristMapMarker : Entity
    {
        public long TouristId { get; private set; }
        public long MapMarkerId { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsStandalone { get; private set; } // false if marker is collected through a tour/encounter/etc, true if predefined

        public TouristMapMarker() { }

        public TouristMapMarker(long touristId, long mapMarkerId)
        {
            TouristId = touristId;
            MapMarkerId = mapMarkerId;
            IsActive = false;
            IsStandalone = false;
        }

        public bool SetMarkerAsActive()
        {
            IsActive = true;
            return IsActive; 
        }

        public bool SetMarkerAsStandalone()
        {
            IsStandalone = true;
            return IsStandalone;
        }
    }
}
