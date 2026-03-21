using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CVision.Controllers.ApiControllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetService<IMediator>()!;

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return ProcessErrors(result);
    }

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return ProcessErrors(result);
    }

    private ActionResult ProcessErrors(ResultBase result)
    {
        var problemsFactory = HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        if (result.HasError(error => error.Message.Contains("not found", StringComparison.CurrentCultureIgnoreCase)))
        {
            return NotFound(problemsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound));
        }

        if (result.HasError(error => error.Message.Equals("Unauthorized")))
        {
            return Unauthorized(problemsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status401Unauthorized));
        }

        var errorDetail = string.Join("; ", result.Errors.Select(e => e.Message));
        return BadRequest(problemsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            detail: errorDetail));
    }
}