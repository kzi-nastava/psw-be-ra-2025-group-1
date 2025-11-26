using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Conversation:Entity
    {

        public long User1Id { get; set; }
        public User User1 { get; set; }

        public long User2Id { get; set; }
        public User User2 { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public DateTime StartedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }

        public Conversation() { }

        public Conversation(long user1Id, long user2Id)
        {
            User1Id = user1Id;
            User2Id = user2Id;
            StartedAt = DateTime.UtcNow;
        }

        public void UpdateLastMessageTime()
        {
            LastMessageAt = DateTime.UtcNow;
        }
    }
}
