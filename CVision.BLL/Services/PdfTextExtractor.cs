using System.Text;
using CVision.BLL.Interfaces;
using UglyToad.PdfPig;

namespace CVision.BLL.Services;

public class PdfTextExtractor : ITextExtractor
{
    public bool CanHandle(string extension)
    {
        return extension.ToLower().Equals(".pdf");
    }

    public async Task<string> ExtractTextAsync(Stream fileStream)
    {
        return await Task.Run(() =>
        {
            var textBuilder = new StringBuilder();
            using (var document = PdfDocument.Open(fileStream))
            {
                foreach (var page in document.GetPages())
                {
                    textBuilder.AppendLine(page.Text);
                }
            }

            return textBuilder.ToString();
        });
    }
}