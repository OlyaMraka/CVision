using CVision.BLL.DTOs.CvAnalyses;
using MediatR;
using CVision.BLL.Helpers;

namespace CVision.BLL.Commands.CvAnalyses.Create;

public record CreateCvAnalysisCommand(CreateCvAnalysisRequestDto RequestDto)
    : IRequest<Result<CvAnalysisResponseDto>>
{
}
