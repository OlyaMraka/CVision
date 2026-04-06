using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using CVision.DAL.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CVision.BLL.Queries.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserResponseDto>>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdHandler(IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<Result<GetUserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return UserConstants.UserNotFound;
        }

        var response = _mapper.Map<GetUserResponseDto>(user);

        return response;
    }
}
