using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;

using CVision.Models.ViewModels.CVAnalysisViewModels;

namespace CVision.Mappers
{
    public class CVAnalysisProfile : Profile
    {
        public CVAnalysisProfile()
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