using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class KeypointProgress : Entity
    {
        public long KeypointId { get; private set; }
        public DateTime ReachedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public KeypointProgress(long keypointId)
        {
            if (keypointId <= 0) throw new ArgumentException("Invalid keypoint ID");

            KeypointId = keypointId;
            ReachedAt = DateTime.UtcNow;
        }

        private KeypointProgress() { }

        public void MarkCompleted()
        {
            if (CompletedAt != null)
                throw new InvalidOperationException("Keypoint already completed");

            CompletedAt = DateTime.UtcNow;
        }

        public bool IsCompleted() => CompletedAt.HasValue;
    }
}
