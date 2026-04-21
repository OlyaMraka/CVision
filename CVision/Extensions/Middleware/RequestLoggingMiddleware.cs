using System.Security.Claims;
using System.Text;

namespace CVision.Extensions.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

        string bodyAsText = await ReadRequestBody(context.Request);

        var logBuilder = new StringBuilder();
        logBuilder.AppendLine("--- Incoming Request ---");
        logBuilder.AppendLine($"User ID: {userId}");
        logBuilder.AppendLine($"Method: {context.Request.Method}");
        logBuilder.AppendLine($"URL: {context.Request.Path}{context.Request.QueryString}");
        logBuilder.AppendLine($"IP: {context.Connection.RemoteIpAddress}");

        logBuilder.AppendLine("Headers:");
        foreach (var header in context.Request.Headers)
        {
            logBuilder.AppendLine($"  {header.Key}: {header.Value}");
        }

        logBuilder.AppendLine($"Body: {bodyAsText}");
        logBuilder.AppendLine("-----------------------");

        _logger.LogInformation(logBuilder.ToString());

        context.Request.Body.Position = 0;

        await _next(context);
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            return string.IsNullOrEmpty(body) ? "Empty" : body;
        }
    }
}