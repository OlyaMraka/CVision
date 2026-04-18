using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Interfaces;
using HtmlAgilityPack;

namespace CVision.BLL.Services;

public class VacancyProvider(IHttpClientService httpClientService) : IVacancyProvider
{
    public async Task<ICollection<VacancyDto>> SearchJobs(string query)
    {
        var url = BuildUrl(query);

        var html = await httpClientService.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new List<VacancyDto>();

        var nodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'l-vacancy')]");

        foreach (var node in nodes)
        {
            var titleNode = node.SelectSingleNode(".//a[contains(@class, 'vt')]");
            var companyNode = node.SelectSingleNode(".//a[contains(@class, 'company')]");

            var urlPath = titleNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;

            result.Add(new VacancyDto
            {
                Title = titleNode?.InnerText.Trim() ?? string.Empty,
                Company = companyNode?.InnerText.Trim() ?? string.Empty,
                Url = urlPath,
                Source = "DOU",
            });
        }

        return result;
    }

    private string BuildUrl(string query)
    {
        return $"https://jobs.dou.ua/vacancies/?search={Uri.EscapeDataString(query)}";
    }
}
