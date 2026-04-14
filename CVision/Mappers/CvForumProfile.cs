using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.Models.ViewModels.CvForum;
using CVision.BLL.DTOs.Comments;

namespace CVision.Mappers;

public class CvForumProfile : Profile
{
    public CvForumProfile()
    {
        CreateMap<PublicationResponseShortDto, PublicationViewModelShort>();
        CreateMap<PublicationResponseShortDto, PublicationsViewModel>();
        CreateMap<PublicationResponseShortDto, ConfirmationViewModal>();
        CreateMap<CommentResponseDto, CommentViewModel>();
        CreateMap<ParentCommentDto, ParentCommentViewModel>();
    }
}
