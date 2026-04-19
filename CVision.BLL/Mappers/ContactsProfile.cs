using AutoMapper;
using CVision.BLL.DTOs.Contacts;
using CVision.DAL.Entities;

namespace CVision.BLL.Mappers;

public class ContactsProfile : Profile
{
    public ContactsProfile()
    {
        CreateMap<Contact, ContactResponseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.ContactUser.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactUser.Email));

        CreateMap<ApplicationUser, UserSearchResultDto>();
    }
}
