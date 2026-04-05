using CVision.BLL.DTOs.Users;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.Users.UpdateProfile;

public record UpdateProfileCommand(int UserId, UpdateProfileRequestDto RequestDto)
    : IRequest<Result<GetUserResponseDto>>;
