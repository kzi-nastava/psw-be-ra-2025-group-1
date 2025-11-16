using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        // Mapiranje Message
        CreateMap<Message, MessageDTO>()
            .ForMember(dest => dest.SenderUsername, opt => opt.MapFrom(src => src.Sender.Username))
            .ForMember(dest => dest.ReceiverUsername, opt => opt.MapFrom(src => src.Receiver.Username));

        CreateMap<MessageDTO, Message>(); // obrnuto mapiranje

        // Mapiranje Conversation
        CreateMap<Conversation, ConversationDTO>()
            .ForMember(dest => dest.User1Username, opt => opt.MapFrom(src => src.User1.Username))
            .ForMember(dest => dest.User2Username, opt => opt.MapFrom(src => src.User2.Username))
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));

        CreateMap<ConversationDTO, Conversation>(); // obrnuto mapiranje
    }
}