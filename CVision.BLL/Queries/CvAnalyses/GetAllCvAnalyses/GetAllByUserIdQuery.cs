using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Helpers;
using MediatR;

namespace CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

public record GetAllByUserIdQuery(int UserId) : IRequest<Result<IEnumerable<CvAnalysisResponseShortDto>>>;
