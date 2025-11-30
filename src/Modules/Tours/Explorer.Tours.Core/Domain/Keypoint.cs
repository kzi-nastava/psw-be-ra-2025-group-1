using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Explorer.Tours.Core.Domain
{
    public class Keypoint : Entity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Secret { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Keypoint(string title, double latitude, double longitude, string description = null, string imageUrl = null, string secret = null)
        {
            Title = title;
            Latitude = latitude;
            Longitude = longitude;
            Description = description;
            ImageUrl = imageUrl;
            Secret = secret;

            Validate();
        }
        
        public Keypoint Update(string title, double latitude, double longitude, string description = null, string imageUrl = null, string secret = null)
        {
            Title = title;
            Latitude = latitude;
            Longitude = longitude;
            Description = description;
            ImageUrl = imageUrl;
            Secret = secret;

            Validate();

            return this;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Invalid title");
            if (double.IsNaN(Latitude) || Latitude < -90 || Latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.");
            if (double.IsNaN(Longitude) || Longitude < -180 || Longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.");

        }
    }
}
