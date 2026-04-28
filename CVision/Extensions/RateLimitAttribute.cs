using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace CVision.Extensions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RateLimitAttribute : ActionFilterAttribute
{
    private readonly int _maxRequests;
    private readonly int _timeWindowInSeconds;

    public RateLimitAttribute(int maxRequests = 10, int timeWindowInSeconds = 60)
    {
        _maxRequests = maxRequests;
        _timeWindowInSeconds = timeWindowInSeconds;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var ipAddress = GetClientIpAddress(context.HttpContext);
        var actionPath = context.ActionDescriptor.DisplayName ?? "unknown";

        var cacheKey = $"RateLimit_{ipAddress}_{actionPath}";

        var cacheEntry = memoryCache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_timeWindowInSeconds);
            return new RateLimitEntry { Count = 0, FirstRequestTime = DateTime.UtcNow };
        });

        if (cacheEntry == null)
        {
            cacheEntry = new RateLimitEntry { Count = 0, FirstRequestTime = DateTime.UtcNow };
        }

        if (DateTime.UtcNow - cacheEntry.FirstRequestTime > TimeSpan.FromSeconds(_timeWindowInSeconds))
        {
            cacheEntry.Count = 1;
            cacheEntry.FirstRequestTime = DateTime.UtcNow;
        }
        else
        {
            cacheEntry.Count++;
        }

        memoryCache.Set(cacheKey, cacheEntry, TimeSpan.FromSeconds(_timeWindowInSeconds));

        if (cacheEntry.Count > _maxRequests)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<RateLimitAttribute>>();
            logger?.LogWarning(
                "Rate limit exceeded for IP {IpAddress} on action {Action}. Requests: {Count}/{Max}",
                ipAddress,
                actionPath,
                cacheEntry.Count,
                _maxRequests);

            context.Result = new RedirectToActionResult("Error", "Errors", new { statusCode = 429 });
            return;
        }

        base.OnActionExecuting(context);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private class RateLimitEntry
    {
        public int Count { get; set; }

        public DateTime FirstRequestTime { get; set; }
    }
}
