using AutoMapper;
using CVision.BLL.DTOs.Analytics;
using CVision.Models.ViewModels.SalaryViewModels;

namespace CVision.Mappers;

public class CVSalaryProfile : Profile
{
    public CVSalaryProfile()
    {
        CreateMap<SalaryRecord, SalaryItemViewModel>();
        CreateMap<PercentileItem, SalaryPercentileViewModel>();
        CreateMap<EmployerInfo, EmployerInfoViewModel>();
        CreateMap<EmployerRatings, EmployerRatingsViewModel>();
        CreateMap<JobTitleInfo, JobTitleInfoViewModel>();
        CreateMap<PayPercentiles, PayPercentilesViewModel>();
        CreateMap<SalaryStats, SalaryStatsViewModel>();
        CreateMap<PercentileItem, SalaryPercentileViewModel>();
    }
}