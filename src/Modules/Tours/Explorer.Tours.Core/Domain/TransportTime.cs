using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{

    /// <summary>
    /// All types of transport available for a tour
    /// </summary>
    public enum TransportType
    {
        Unknown,
        Foot,
        Bike,
        Car,
    }

    public class TransportTime : Entity
    {
        /// <summary>
        /// Gets the transport type used by this instance.
        /// </summary>
        public TransportType Type { get; private set; }

        /// <summary>
        /// Minutes required for the transport type
        /// </summary>
        public int Duration { get; private set; }

        public TransportTime()
        {
            Type = TransportType.Foot;
            Duration = 0;
        }

        public TransportTime(TransportType type, int duration)
        {
            Type = type;
            Duration = duration;
            Validate();
        }

        public TransportTime Update(TransportTime time)
        {
            Duration=time.Duration;
            Type=time.Type;

            Validate();
            return this;
        }


        private bool Validate()
        {
            if (Duration < 0) throw new ArgumentException("Duration cannot be negative.");
            if(Type == TransportType.Unknown) throw new ArgumentException("Transport type cannot be unknown.");
            return true;
        }
    }
}
