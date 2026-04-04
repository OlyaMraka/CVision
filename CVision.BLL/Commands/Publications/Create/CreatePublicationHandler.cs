using MediatR;
using AutoMapper;
using CVision.BLL.Constans;
using CVision.BLL.Helpers;
using FluentValidation;
using CVision.BLL.Interfaces;
using CVision.BLL.DTOs.Publications;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;

namespace CVision.BLL.Commands.Publications.Create;

public class CreatePublicationHandler(
    IValidator<CreatePublicationCommand> validator,
    IMapper mapper,
    IFileService fileService,
    IRepositoryWrapper repositoryWrapper) : IRequestHandler<CreatePublicationCommand, Result<CreatePublicationResponseDto>>
{
    public async Task<Result<CreatePublicationResponseDto>> Handle(
        CreatePublicationCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        using var ms = new MemoryStream();
        await request.Request.FileStream.CopyToAsync(ms);
        byte[] fileBytes = ms.ToArray();

        using var uploadStream = new MemoryStream(fileBytes);
        var (filePath, publicId) = await fileService.UploadFileAsync(
            uploadStream, request.Request.FileName);

        var cv = new CV
        {
            UserId = request.Request.UserId,
            FilePath = filePath,
            PublicId = publicId,
            UploadedAt = DateTime.UtcNow,
        };

        await repositoryWrapper.CvRepository.CreateAsync(cv);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return PublicationsConstants.CvSaveError;
        }

        var newPublication = mapper.Map<Publication>(request.Request);
        newPublication.CVId = cv.Id;

        await repositoryWrapper.PublicationRepository.CreateAsync(newPublication);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return PublicationsConstants.PublicationSaveError;
        }

        var response = mapper.Map<CreatePublicationResponseDto>(newPublication);

        return response;
    }
}