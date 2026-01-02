using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class Wallet: Entity
    {
        public long TouristId { get; private set; }
        public double Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Wallet()
        {
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public Wallet(long touristId, double balance=0)
        {
            TouristId= touristId;
            Balance = balance;
        }

        public void Update(double balance)
        {
            Balance= balance;
        }
    }
}
