using AutoMapper;
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
    }
}