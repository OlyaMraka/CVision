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
    }
}