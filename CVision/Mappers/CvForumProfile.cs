using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.Models.ViewModels.CvForum;

namespace CVision.Mappers;

public class CvForumProfile : Profile
{
    public CvForumProfile()
    {
        CreateMap<PublicationResponseShortDto, PublicationViewModelShort>();
    }
}