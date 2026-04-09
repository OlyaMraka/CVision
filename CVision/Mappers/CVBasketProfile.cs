using AutoMapper;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.Models.ViewModels.CVBasketViewModels;

namespace CVision.Mappers;

public class CVBasketProfile : Profile
{
    public CVBasketProfile()
    {
        CreateMap<DeletedCvAnalysisResponseDto, CVBasketItemViewModel>();
    }
}