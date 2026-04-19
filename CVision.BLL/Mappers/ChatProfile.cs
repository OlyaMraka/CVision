using AutoMapper;
using CVision.BLL.DTOs.Chat;
using CVision.DAL.Entities;

namespace CVision.BLL.Mappers;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.UserName))
            .ForMember(dest => dest.ReceiverUserName, opt => opt.MapFrom(src => src.Receiver.UserName));
    }
}
