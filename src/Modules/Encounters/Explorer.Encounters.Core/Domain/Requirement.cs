using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class Requirement : Entity
    {
        public string Description { get; private set; }
        public bool IsMet { get; private set; }
        public long ActiveEncounterId { get; private set; }
        
        public Requirement(string description, long activeEncounterId)
        {
            Description = description;
            IsMet = false;
            ActiveEncounterId = activeEncounterId;
        }
        
        private Requirement() { }

        public void MarkAsMet()
        {
            IsMet = true;
        }

        public void Reset()
        {
            IsMet = false;
        }
    }
}
