using AutoMapper;
using CVision.BLL.DTOs.Comments;
using CVision.DAL.Entities;

namespace CVision.BLL.Mappers;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentRequestDto, Comment>();

        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

        CreateMap<Comment, ParentCommentDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
    }
}