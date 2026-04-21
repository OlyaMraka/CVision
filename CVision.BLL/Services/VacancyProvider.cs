using CVision.BLL.DTOs.Vacancies;
using CVision.BLL.Interfaces;
using CVision.BLL.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CVision.BLL.Services;

public class VacancyProvider(
    IHttpClientService httpClientService,
    IMemoryCache cache,
    IOptions<CacheOptions> cacheOptions) : IVacancyProvider
{
    public async Task<ICollection<VacancyDto>> SearchJobs(string query)
    {
        var cacheKey = $"dou:{query.ToLower()}";

        if (cache.TryGetValue(cacheKey, out List<VacancyDto>? cached))
        {
            return cached!;
        }

        var url = BuildUrl(query);

        var html = await httpClientService.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var result = new List<VacancyDto>();

        var nodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'l-vacancy')]");

        if (nodes is null)
        {
            return result;
        }

        foreach (var node in nodes)
        {
            var titleNode = node.SelectSingleNode(".//a[contains(@class, 'vt')]");
            var companyNode = node.SelectSingleNode(".//a[contains(@class, 'company')]");

            var urlPath = titleNode?.GetAttributeValue("href", string.Empty) ?? string.Empty;

            result.Add(new VacancyDto
            {
                Title = HtmlEntity.DeEntitize(titleNode?.InnerText.Trim() ?? string.Empty),
                Company = HtmlEntity.DeEntitize(companyNode?.InnerText.Trim() ?? string.Empty),
                Url = urlPath,
                Source = "DOU",
            });
        }

        cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheOptions.Value.VacanciesCacheMinutes,
        });

        return result;
    }

    private string BuildUrl(string query)
    {
        return $"https://jobs.dou.ua/vacancies/?search={Uri.EscapeDataString(query)}";
    }
}
