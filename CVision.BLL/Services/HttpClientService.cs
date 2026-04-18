using CVision.BLL.Interfaces;

namespace CVision.BLL.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _client;

    public HttpClientService(HttpClient client)
    {
        _client = client;

        _client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        _client.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<string> GetStringAsync(string url)
    {
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        return response;
    }
}
