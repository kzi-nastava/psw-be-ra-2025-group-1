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
        private readonly IMapper _mapper;

        public MessageService(
            IMessageRepository messageRepository,
            IConversationRepository conversationRepository,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _mapper = mapper;
        }

        public async Task<MessageDTO> SendMessageAsync(long senderId, long receiverId, string content)
        {
            // pronađi ili kreiraj konverzaciju
            var conversation = await _conversationRepository.GetOrCreateConversationAsync(senderId, receiverId);

            var message = new Message(senderId, receiverId, conversation.Id, content);
            await _messageRepository.AddAsync(message);

            conversation.UpdateLastMessageTime();
            await _conversationRepository.UpdateAsync(conversation);

            return _mapper.Map<MessageDTO>(message);
        }

        public async Task<IEnumerable<ConversationDTO>> GetUserConversationsAsync(long userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
            return _mapper.Map<IEnumerable<ConversationDTO>>(conversations);
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
