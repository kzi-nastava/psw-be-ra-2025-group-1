using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Task AddAsync(Message message);
        Task<Message> GetByIdAsync(long messageId);
        Task<IEnumerable<Message>> GetByConversationIdAsync(long conversationId);
        Task UpdateAsync(Message message);
        Task DeleteAsync(Message message);

    }
}
