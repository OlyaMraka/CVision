namespace CVision.BLL.Interfaces;

public interface IHttpClientService
{
    Task<string> GetStringAsync(string url);

    Task<HttpResponseMessage> GetAsync(string url);

    Task<T?> GetFromJsonAsync<T>(string url, IDictionary<string, string>? headers = null);
}