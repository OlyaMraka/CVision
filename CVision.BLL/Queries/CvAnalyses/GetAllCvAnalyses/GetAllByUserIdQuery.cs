using CVision.BLL.DTOs.CvAnalyses;
using FluentResults;
using MediatR;

namespace CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;

public record GetAllByUserIdQuery(int UserId) : IRequest<Result<IEnumerable<CvAnalysisResponseShortDto>>>;