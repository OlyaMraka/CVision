namespace CVision.BLL.Interfaces;

public interface IFileService
{
    Task<(string FilePath, string PublicId)> UploadFileAsync(Stream fileStream, string fileName);

    Task<bool> DeleteFileAsync(string publicId);
}