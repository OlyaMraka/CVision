using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Commands.CvAnalyses.Recover;

public record RecoverCvAnalysisCommand(int CvAnalysisId)
    : IRequest<Result<bool>>;
