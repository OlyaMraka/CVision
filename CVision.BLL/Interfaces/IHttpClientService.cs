namespace CVision.BLL.Interfaces;

public interface IHttpClientService
{
    Task<string> GetStringAsync(string url);

    Task<HttpResponseMessage> GetAsync(string url);
}