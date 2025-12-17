using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.Domain
{
    public class Monument : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int CreationYear { get; private set; }
        public MonumentStatus Status { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public Monument() { }

        public Monument(string name, string description, double longitude, double latitude)
        {
            Name = name;
            Description = description;
            CreationYear = DateTime.Now.Year;
            Status = MonumentStatus.Active;
            Longitude = longitude;
            Latitude = latitude;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(Name)) throw new EntityValidationException("Name cannot be empty.");
            if (string.IsNullOrEmpty(Description)) throw new EntityValidationException("Description cannot be empty.");
            if (Longitude < -180 || Longitude > 180) throw new EntityValidationException("Invalid longitude.");
            if (Latitude < -90 ||  Latitude > 90) throw new EntityValidationException("Invalid latitude.");
        }
    }
}
