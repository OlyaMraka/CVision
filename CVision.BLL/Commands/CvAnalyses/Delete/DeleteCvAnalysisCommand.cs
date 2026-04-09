using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.CvAnalyses.Delete;

public record DeleteCvAnalysisCommand(int CvAnalysisId)
    : IRequest<Result<bool>>;