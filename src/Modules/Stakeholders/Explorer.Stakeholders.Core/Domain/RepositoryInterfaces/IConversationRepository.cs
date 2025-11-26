using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IConversationRepository
    {
        Task<Conversation> GetOrCreateConversationAsync(long user1Id, long user2Id);
        Task UpdateAsync(Conversation conversation);
        Task<Conversation> GetByIdAsync(long conversationId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(long userId);
    }
}
