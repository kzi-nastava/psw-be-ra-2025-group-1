using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class MessageDTO
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public long ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}
