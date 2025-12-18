using AutoMapper;

using Explorer.Stakeholders.API.Dtos;      //  DTO-i
using Explorer.Stakeholders.Core.Domain;   //  Rating domen


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
        CreateMap<PersonDto, Person>().ReverseMap();

        CreateMap<Rating, RatingDto>();     //domain -> dto
        CreateMap<RatingCreateDto, Rating>();
        CreateMap<RatingUpdateDto, Rating>();
        
        CreateMap<Journal, JournalDto>();
        CreateMap<JournalCreateDto, Journal>();
        CreateMap<JournalUpdateDto, Journal>();

        
        CreateMap<UserLocationDto, UserLocation>().ReverseMap();
        
        CreateMap<ProblemDto, Problem>()
            .ConstructUsing(dto => new Problem(
                dto.Priority,
                dto.Description,
                (Core.Domain.ProblemCategory)dto.Category,
                dto.TourId,
                dto.CreatorId,
                dto.AuthorId
            ));
        
        CreateMap<Problem, ProblemDto>()
            .ForMember(dest => dest.LateFlag, opt => opt.MapFrom(src => src.IsOverdue()));
        
        CreateMap<Core.Domain.ProblemStatus, API.Dtos.ProblemStatus>().ReverseMap();
        CreateMap<Core.Domain.ProblemCategory, API.Dtos.ProblemCategory>().ReverseMap();
        
        // Mapiranje ProblemMessage
        CreateMap<ProblemMessage, ProblemMessageDto>().ReverseMap();
        
        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<NotificationTypeDto, NotificationType>().ReverseMap();
    }
}