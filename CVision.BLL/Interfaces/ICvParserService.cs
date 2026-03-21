namespace CVision.BLL.Interfaces;

public interface ICvParserService
{
    Task<string> ParseAsync(Stream fileStream, string fileName);
}