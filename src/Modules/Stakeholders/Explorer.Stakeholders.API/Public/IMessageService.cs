using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        Task<MessageDTO> SendMessageAsync(long senderId, long receiverId, string content);
        Task<IEnumerable<ConversationDTO>> GetUserConversationsAsync(long userId);
        Task<IEnumerable<MessageDTO>> GetConversationMessagesAsync(long conversationId);
        Task EditMessageAsync(long messageId, string newContent);
        Task DeleteMessageAsync(long messageId);
    }
}
