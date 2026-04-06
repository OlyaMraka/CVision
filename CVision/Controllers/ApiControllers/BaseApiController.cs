using CVision.BLL.Helpers;
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

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var problemsFactory = HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        if (result.Error!.Contains("not found", StringComparison.CurrentCultureIgnoreCase))
        {
            return NotFound(problemsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status404NotFound));
        }

        if (result.Error!.Equals("Unauthorized"))
        {
            return Unauthorized(problemsFactory.CreateProblemDetails(HttpContext, statusCode: StatusCodes.Status401Unauthorized));
        }

        return BadRequest(problemsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            detail: result.Error));
    }
}
