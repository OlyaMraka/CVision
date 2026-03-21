using CVision.BLL.Interfaces;

namespace CVision.BLL.Services;

public class ImageTextExtractor : ITextExtractor
{
    private readonly string _dataPath;

    public ImageTextExtractor()
    {
        _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
    }

    public bool CanHandle(string extension)
    {
        string ext = extension.ToLower();
        return ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg");
    }

    public async Task<string> ExtractTextAsync(Stream fileStream)
    {
        return await Task.Run(() =>
        {
            using (var engine = new Tesseract.TesseractEngine(_dataPath, "ukr+eng", Tesseract.EngineMode.Default))
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                using (var img = Tesseract.Pix.LoadFromMemory(fileBytes))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        });
    }
}