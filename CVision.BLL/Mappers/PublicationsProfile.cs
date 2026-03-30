using AutoMapper;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;

namespace CVision.BLL.Mappers;

public class PublicationsProfile : Profile
{
    public PublicationsProfile()
    {
        CreateMap<CreatePublicationRequestDto, Publication>();

        CreateMap<Publication, CreatePublicationResponseDto>()
            .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(x => x.CV!.FilePath));

        CreateMap<Publication, PublicationResponseShortDto>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(x => x.CV!.FilePath))
            .ForMember(dest => dest.CreatorUserName, opt => opt.MapFrom(x => x.User.UserName));
    }
}