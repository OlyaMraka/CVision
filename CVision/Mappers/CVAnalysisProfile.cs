using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;

using CVision.Models.ViewModels.CVAnalysisViewModels;

namespace CVision.Mappers
{
    public class CvAnalysisProfile : Profile
    {
        public CvAnalysisProfile()
        {
            CreateMap<CvSectionAnalisysResultDto, CVSectionResultViewModel>();

            // 🔹 Main model
            CreateMap<CvAnalysisResultDto, CVAnalysisViewModel>()
                .ForMember(
                    dest => dest.SectionResults,
                    opt => opt.MapFrom(src => src.SectionsResults));
        }
    }
}