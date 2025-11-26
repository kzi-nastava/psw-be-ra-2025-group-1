using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly StakeholdersContext _context;

        public ConversationRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(long user1Id, long user2Id)
        {
            if (user1Id == user2Id)
                throw new Exception("User cannot create conversation with himself.");

            if (user1Id > user2Id)
                (user1Id, user2Id) = (user2Id, user1Id);

            var conversation = await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.User1Id == user1Id && c.User2Id == user2Id);

            if (conversation == null)
            {
                conversation = new Conversation(user1Id, user2Id)
                {
                    StartedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow
                };

                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();

                // Load FK navigation properties
                await _context.Entry(conversation).Reference(c => c.User1).LoadAsync();
                await _context.Entry(conversation).Reference(c => c.User2).LoadAsync();
            }

            return conversation;
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<Conversation> GetByIdAsync(long conversationId)
        {
            return await _context.Conversations.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(long userId)
        {
            return await _context.Conversations
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages)
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .OrderByDescending(c => c.LastMessageAt ?? c.StartedAt)
                .ToListAsync();
        }


    }
}
