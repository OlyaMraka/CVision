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
                CreateMap<CvAnalysisResponseShortDto, CVGalleryViewModel>();
                CreateMap<CvAnalysisResultDto, CVAnalysisResultViewModel>();
                CreateMap<CvAnalysisResponseDto, CVAnalysisViewModel>();
                CreateMap<CvAnalysisResponseShortDto, CVAnalysisConfirmationViewModel>();
                CreateMap<CvAnalysisInfoResponseDto, CVAnalysisInfoViewModel>();
            }
        }
    }