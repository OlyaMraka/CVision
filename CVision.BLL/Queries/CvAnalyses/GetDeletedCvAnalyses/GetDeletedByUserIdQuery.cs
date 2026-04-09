using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;

public record GetDeletedByUserIdQuery(int UserId)
    : IRequest<Result<IEnumerable<DeletedCvAnalysisResponseDto>>>;