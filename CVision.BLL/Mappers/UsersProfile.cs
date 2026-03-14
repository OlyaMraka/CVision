using AutoMapper;
using CVision.BLL.DTOs.Users;
using CVision.DAL.Entities;

namespace CVision.BLL.Mappers;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<RegisterUserRequestDto, ApplicationUser>();
        CreateMap<ApplicationUser, RegisterUserResponseDto>();
    }
}