namespace CVision.BLL.Interfaces;

public interface ITextExtractor
{
    bool CanHandle(string extension);

    Task<string> ExtractTextAsync(Stream fileStream);
}