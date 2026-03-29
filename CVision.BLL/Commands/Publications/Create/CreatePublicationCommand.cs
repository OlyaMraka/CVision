using CVision.BLL.DTOs.Publications;
using MediatR;
using FluentResults;

namespace CVision.BLL.Commands.Publications.Create;

public record CreatePublicationCommand(CreatePublicationRequestDto Request)
    : IRequest<Result<CreatePublicationResponseDto>>;