using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CVision.BLL.Interfaces;
using CVision.BLL.Options;

namespace CVision.BLL.Services;

public class CloudinaryFileService : IFileService
{
    private const string FolderName = "cvision-resumes";

    private readonly Cloudinary _cloudinary;

    public CloudinaryFileService(CloudinaryOptions options)
    {
        var account = new Account(
            options.CloudName,
            options.ApiKey,
            options.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string FilePath, string PublicId)> UploadFileAsync(Stream fileStream, string fileName)
    {
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = FolderName,
            PublicId = $"{Guid.NewGuid()}_{Path.GetFileNameWithoutExtension(fileName)}",
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            throw new Exception($"Cloudinary Upload Error: {result.Error.Message}");
        }

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<bool> DeleteFileAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return false;
        }

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Raw,
        };

        var result = await _cloudinary.DestroyAsync(deletionParams);

        return result.Result.Equals("ok") || result.Result.Equals("not found");
    }
}
