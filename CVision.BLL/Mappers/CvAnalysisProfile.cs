using AutoMapper;
using CVision.BLL.Constans;
using CVision.DAL.Entities;
using CVision.BLL.DTOs.CvAnalyses;

namespace CVision.BLL.Mappers;

public class CvAnalysisProfile : Profile
{
    public CvAnalysisProfile()
    {
        CreateMap<CvAnalysisResultDto, CVAnalysis>()
            .ForMember(dest => dest.Recommendations, opt => opt.MapFrom(src => src.SectionsResults));

        CreateMap<CvSectionAnalisysResultDto, CVAnalysisRecommendation>();

        CreateMap<CVAnalysis, CvAnalysisResponseShortDto>()
            .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(x => x.CV.FilePath));

        CreateMap<CVAnalysis, DeletedCvAnalysisResponseDto>()
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(x => x.CV.FilePath))
            .ForMember(dest => dest.Days,
                opt => opt.MapFrom(x =>
                   CvAnalysisConstants.DaysAlive - (DateTime.UtcNow - x.DeletedAt!.Value).TotalDays));
    }
}