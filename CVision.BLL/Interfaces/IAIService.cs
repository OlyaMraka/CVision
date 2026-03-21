using CVision.BLL.DTOs.CvAnalyses;

namespace CVision.BLL.Interfaces;

public interface IAIService
{
    Task<CvAnalysisResultDto> AnalyzeResumeAsync(string rawText);
}