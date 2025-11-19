using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public MessageService(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    IUserRepository userRepository,
    IMapper mapper)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<MessageDTO> SendMessageAsync(long senderId, long receiverId, string content)
        {
            var conversation = await _conversationRepository.GetOrCreateConversationAsync(senderId, receiverId);

            var message = new Message(senderId, receiverId, conversation.Id, content);
            await _messageRepository.AddAsync(message);

            conversation.UpdateLastMessageTime();
            await _conversationRepository.UpdateAsync(conversation);

            var loadedMessage = await _messageRepository.GetByIdAsync(message.Id);

            return _mapper.Map<MessageDTO>(loadedMessage);
        }


        public async Task<IEnumerable<ConversationDTO>> GetUserConversationsAsync(long userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
            var dtoList = new List<ConversationDTO>();

            foreach (var c in conversations)
            {
                var lastMsg = c.Messages
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefault();

                var otherUser = c.User1Id == userId ? c.User2 : c.User1;

                dtoList.Add(new ConversationDTO
                {
                    Id = c.Id,

                    // 🔥 Dodato – OVO JE NEDOSTAJALO
                    User1Id = c.User1Id,
                    User1Username = c.User1?.Username,
                    User2Id = c.User2Id,
                    User2Username = c.User2?.Username,

                    OtherUserName = otherUser?.Username,
                    LastMessage = lastMsg?.Content ?? "No messages",
                    LastMessageAt = lastMsg?.Timestamp
                });
            }

            return dtoList.OrderByDescending(c => c.LastMessageAt ?? DateTime.MinValue);
        }




        public async Task<IEnumerable<MessageDTO>> GetConversationMessagesAsync(long conversationId)
        {
            var messages = await _messageRepository.GetByConversationIdAsync(conversationId);
            return _mapper.Map<IEnumerable<MessageDTO>>(messages);
        }

        public async Task EditMessageAsync(long messageId, string newContent)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.IsDeleted)
                throw new Exception("Message not found or has been deleted.");

            message.Content = newContent;
            message.EditedAt = DateTime.UtcNow;

            await _messageRepository.UpdateAsync(message);
        }

        public async Task DeleteMessageAsync(long messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            if (message == null || message.IsDeleted)
                throw new Exception("Message not found or already deleted.");

            // Soft delete
            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;

            await _messageRepository.UpdateAsync(message);
        }
    }
}
