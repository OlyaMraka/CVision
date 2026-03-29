using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.Models.ViewModels.CVForumViewModels;

namespace CVision.Mappers;

public class CVForumProfile : Profile
{
    public CVForumProfile()
    {
        // 🔹 ViewModel → Request DTO
        CreateMap<CreateCVForumPostViewModel, CreatePublicationRequestDto>()
            .ForMember(dest => dest.FileStream,
                opt => opt.MapFrom(src => src.File.OpenReadStream()))
            .ForMember(dest => dest.FileName,
                opt => opt.MapFrom(src => src.File.FileName))
            .ForMember(dest => dest.ContentType,
                opt => opt.MapFrom(src => src.File.ContentType))
            .ForMember(dest => dest.UserId,
                opt => opt.Ignore());


        // 🔹 Response DTO → ViewModel
        CreateMap<CreatePublicationResponseDto, CVForumPostViewModel>()
            .ForMember(dest => dest.Views,
                opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.Comments,
                opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.AuthorName,
                opt => opt.Ignore())
            .ForMember(dest => dest.AuthorRole,
                opt => opt.Ignore())
            .ForMember(dest => dest.IsOwner,
                opt => opt.Ignore());
    }
}