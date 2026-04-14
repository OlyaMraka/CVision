using System.IO;

namespace CVision.Helpers;

public static class FileUtils
{
    private static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp",
    };

    private static readonly HashSet<string> DocxExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".docx",
        ".doc",
    };

    public static bool IsImage(string url) =>
        !string.IsNullOrEmpty(url) && ImageExtensions.Contains(GetPureExtension(url));

    public static bool IsDocx(string url) =>
        !string.IsNullOrEmpty(url) && DocxExtensions.Contains(GetPureExtension(url));

    public static bool IsPdf(string url) =>
        !string.IsNullOrEmpty(url) && string.Equals(GetPureExtension(url), ".pdf", StringComparison.OrdinalIgnoreCase);

    private static string GetPureExtension(string url)
    {
        var path = url.Split('?', '#')[0];
        return Path.GetExtension(path);
    }
}
