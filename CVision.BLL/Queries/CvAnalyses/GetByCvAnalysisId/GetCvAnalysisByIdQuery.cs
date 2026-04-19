using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.CvAnalyses.GetByCvAnalysisId;

public record GetCvAnalysisByIdQuery(int Id)
    : IRequest<Result<CvAnalysisInfoResponseDto>>;
