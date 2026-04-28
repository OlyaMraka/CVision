using System.Net.Http.Json;
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

    public async Task<T?> GetFromJsonAsync<T>(string url, IDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
