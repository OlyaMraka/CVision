using AutoMapper;
using MediatR;
using FluentResults;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Interfaces;
using CVision.DAL.Entities;
using CVision.DAL.Repositories.Interfaces.Base;

namespace CVision.BLL.Commands.CvAnalyses.Create;

public class CreateCvAnalysisHandler(
    IAIService aiService,
    ICvParserService cvParser,
    IFileService fileService,
    IRepositoryWrapper repositoryWrapper,
    IMapper mapper) : IRequestHandler<CreateCvAnalysisCommand, Result<CvAnalysisResultDto>>
{
    public async Task<Result<CvAnalysisResultDto>> Handle(
        CreateCvAnalysisCommand request,
        CancellationToken cancellationToken)
    {
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
            return Result.Fail<CvAnalysisResultDto>("Помилка збереження файлу");
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

        if (await repositoryWrapper.SaveChangesAsync() <= 0)
        {
            return Result.Fail<CvAnalysisResultDto>("Помилка збереження результатів аналізу");
        }

        return Result.Ok(aiResult);
    }
}