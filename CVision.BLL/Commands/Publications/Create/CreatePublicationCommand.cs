using CVision.BLL.DTOs.Publications;
using CVision.BLL.Helpers;
using MediatR;
using FluentResults;

namespace CVision.BLL.Commands.Publications.Create;

public record CreatePublicationCommand(CreatePublicationRequestDto Request)
    : IRequest<MyResult<CreatePublicationResponseDto>>;