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
        public string Title { get; init; }
        public string? Description { get; init; }
        public string? ImageUrl { get; init; }
        public string? Secret { get; init; }

        public Keypoint(string title, string description = null, string imageUrl = null, string secret = null)
        {
            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            Secret = secret;

            Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Title)) throw new ArgumentException("Invalid title");
        }
    }
}
