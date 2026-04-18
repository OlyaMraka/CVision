using AutoMapper;
using CVision.BLL.Constans;
using MediatR;
using CVision.BLL.Helpers;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;
using FluentValidation;

namespace CVision.BLL.Commands.CvAnalyses.Create;

public class CreateCvAnalysisHandler(
    IAIService aiService,
    ICvParserService cvParser,
    IFileService fileService,
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper,
    IValidator<CreateCvAnalysisCommand> validator) : IRequestHandler<CreateCvAnalysisCommand, Result<CvAnalysisResponseDto>>
{
    public async Task<Result<CvAnalysisResponseDto>> Handle(
        CreateCvAnalysisCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.Errors.First().ErrorMessage;
        }

        using var ms = new MemoryStream();
        await request.RequestDto.FileStream.CopyToAsync(ms);
        byte[] fileBytes = ms.ToArray();

        using var uploadStream = new MemoryStream(fileBytes);
        var (filePath, publicId) = await fileService.UploadFileAsync(
            uploadStream, request.RequestDto.FileName);

        var cv = new CV
        {
            UserId = request.RequestDto.UserId,
            FilePath = filePath,
            PublicId = publicId,
            UploadedAt = DateTime.UtcNow,
        };

        await repositoryWrapper.CvRepository.CreateAsync(cv);

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CvAnalysisConstants.CvSavingError;
        }

        if (request.RequestDto.FileStream.CanSeek)
        {
            request.RequestDto.FileStream.Position = 0;
        }

        using var parseStream = new MemoryStream(fileBytes);
        string rawText = await cvParser.ParseAsync(
            parseStream,
            request.RequestDto.FileName);

        var aiResult = await aiService.AnalyzeResumeAsync(rawText);

        var cvAnalysis = mapper.Map<CVAnalysis>(aiResult);
        cvAnalysis.CVId = cv.Id;

        await repositoryWrapper.CvAnalysisRepository.CreateAsync(cvAnalysis);
        foreach (var lookup in aiResult.CvLookups)
        {
            var cvLookup = new CvLookup
            {
                CvId = cv.Id,
                LookupWord = lookup,
            };

            await repositoryWrapper.CvLookupRepository.CreateAsync(cvLookup);
        }

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return CvAnalysisConstants.CvAnalysisSavingError;
        }

        var response = new CvAnalysisResponseDto
        {
            Id = cvAnalysis.Id,
            FileUrl = cv.FilePath,
            AnalysisResult = aiResult,
        };

        return response;
    }
}