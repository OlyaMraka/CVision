using CVision.BLL.DTOs.CvAnalyses;
using MediatR;
using FluentResults;

namespace CVision.BLL.Commands.CvAnalyses.Create;

public record CreateCvAnalysisCommand(CreateCvAnalysisRequestDto RequestDto)
    : IRequest<Result<CvAnalysisResultDto>>
{
}
