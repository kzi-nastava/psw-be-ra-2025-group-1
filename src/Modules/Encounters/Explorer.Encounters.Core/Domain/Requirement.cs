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
        public long ActiveEncounterId { get; private set; }
        public string Description { get; private set; }
        public bool IsMet { get; private set; }
        public Requirement(long encounterId, string description)
        {
            ActiveEncounterId = encounterId;
            Description = description;
            IsMet = false;
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
