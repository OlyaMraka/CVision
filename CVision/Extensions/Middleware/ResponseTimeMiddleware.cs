using System.Diagnostics;

namespace CVision.Extensions.Middleware;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseTimeMiddleware> _logger;

    public ResponseTimeMiddleware(RequestDelegate next, ILogger<ResponseTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();

        await _next(context);

        watch.Stop();

        var elapsedMs = watch.ElapsedMilliseconds;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var statusCode = context.Response.StatusCode;

        if (elapsedMs > 500)
        {
            _logger.LogWarning(
                "Slow Request: {Method} {Path} responded {StatusCode} in {Elapsed} ms",
                method,
                path,
                statusCode,
                elapsedMs);
        }
        else
        {
            _logger.LogInformation(
                "Request: {Method} {Path} responded {StatusCode} in {Elapsed} ms",
                method,
                path,
                statusCode,
                elapsedMs);
        }
    }
}