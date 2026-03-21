using DocumentFormat.OpenXml.Packaging;
using System.Text;
using CVision.BLL.Interfaces;
using UglyToad.PdfPig.DocumentLayoutAnalysis.Export;

namespace CVision.BLL.Services;

public class DocxTextExtractor : ITextExtractor
{
    public bool CanHandle(string extension)
    {
        return extension.ToLower().Equals(".docx") || extension.ToLower().Equals(".doc");
    }

    public async Task<string> ExtractTextAsync(Stream fileStream)
    {
        return await Task.Run(() =>
        {
            using var doc = WordprocessingDocument.Open(fileStream, false);
            return doc.MainDocumentPart?.Document!.Body?.InnerText ?? string.Empty;
        });
    }
}