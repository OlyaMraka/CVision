using CVision.BLL.Interfaces;

namespace CVision.BLL.Services;

public class CvParserService : ICvParserService
{
    private readonly IEnumerable<ITextExtractor> _extractors;

    public CvParserService(IEnumerable<ITextExtractor> extractors)
    {
        _extractors = extractors;
    }

    public async Task<string> ParseAsync(Stream fileStream, string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

        ITextExtractor? extractor = _extractors.FirstOrDefault(e => e.CanHandle(extension));

        if (extractor == null)
        {
            throw new NotSupportedException($"Формат файлу {extension} не підтримується системою.");
        }

        return await extractor.ExtractTextAsync(fileStream);
    }
}