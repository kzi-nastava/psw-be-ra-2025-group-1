using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Message
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public User Sender { get; set; }
        public long ReceiverId   { get; set; }
        public User Receiver {  get; set; }

        public long ConversationId {  get; set; }
        public Conversation Conversation { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? EditedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
       
        public Message() { }
        public Message(long senderId, long receiverId, long conversationId, string content)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            ConversationId = conversationId;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }
    }
}
