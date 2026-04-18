using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Commands.CvAnalyses.Create;
using CVision.BLL.Commands.CvAnalyses.Delete;
using CVision.BLL.Queries.CvAnalyses.GetAllCvAnalyses;
using CVision.BLL.Queries.CvAnalyses.GetByCvAnalysisId;
using CVision.BLL.Queries.CvAnalyses.GetDeletedCvAnalyses;
using CVision.BLL.Queries.Vacancies;
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

        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var requestDto = new CreateCvAnalysisRequestDto
        {
            FileStream = memoryStream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            UserId = userId,
        };

        using (memoryStream)
        {
            return Ok(await Mediator.Send(new CreateCvAnalysisCommand(requestDto)));
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CvAnalysisResponseShortDto))]
    public async Task<IActionResult> GetAllByUserId([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetAllByUserIdQuery(id)));
    }

    [HttpDelete("delete/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCvAnalysis([FromRoute] int id)
    {
        return Ok(await Mediator.Send(new DeleteCvAnalysisCommand(id)));
    }

    [HttpGet("deleted/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeleted([FromRoute] int id)
    {
        return Ok(await Mediator.Send(new GetDeletedByUserIdQuery(id)));
    }

    [HttpGet("vacancies/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCvVacancies([FromRoute] int id)
    {
        return Ok(await Mediator.Send(new GetByCvIdQuery(id)));
    }

    [HttpGet("cv-analysis/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCvAnalysisById([FromRoute] int id)
    {
        return Ok(await Mediator.Send(new GetCvAnalysisByIdQuery(id)));
    }
}