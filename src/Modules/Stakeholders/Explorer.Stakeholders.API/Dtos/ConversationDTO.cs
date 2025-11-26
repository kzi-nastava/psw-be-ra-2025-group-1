using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ConversationDTO
    {
        public long Id { get; set; }
        public long User1Id { get; set; }
        public string User1Username { get; set; }
        public long User2Id { get; set; }
        public string User2Username { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }

        public string OtherUserName { get; set; }
        public string LastMessage { get; set; }

        public List<MessageDTO> Messages { get; set; } = new();
    }

}
