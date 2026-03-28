using System.ComponentModel.DataAnnotations;
using CVision.BLL.DTOs.CvAnalyses;

namespace CVision.Models.ViewModels.CVAnalysisViewModels;

public class CVSectionResultViewModel
{
    [Display(Name = "Розділ")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Вміст")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Оцінка")]
    public int SectionScore { get; set; }
}