using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Commands.CvAnalyses.Create;
using Microsoft.AspNetCore.Mvc;

namespace CVision.Controllers.ApiControllers;

public class CvAnalysisController : BaseApiController
{
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeCv([FromForm] IFormFile file, [FromForm] int userId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Файл не вибрано.");
        }

        // Копіюємо файл у пам'ять повністю
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Скидаємо на початок перед відправкою

        var requestDto = new CreateCvAnalysisRequestDto
        {
            FileStream = memoryStream, // Тепер це MemoryStream, він не "здохне" завчасно
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
        };

        // ВАЖЛИВО: MemoryStream треба закрити ПІСЛЯ завершення всієї команди
        using (memoryStream)
        {
            return HandleResult(await Mediator.Send(new CreateCvAnalysisCommand(requestDto)));
        }
    }
}