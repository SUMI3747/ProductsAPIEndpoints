using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ProductInventoryManagerAPI.ActionFilters
{
    public class RateLimitFilter : IActionFilter
    {
        private readonly IMemoryCache _cache;
        private readonly int _requestLimit = 4;
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

        public RateLimitFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var clientKey = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(clientKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Client key could not be determined."
                };
                return;
            }

            
            var requestCounter = _cache.GetOrCreate(clientKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _timeWindow;
                return new RequestCounter { Count = 0, Timestamp = DateTime.UtcNow };
            });

            if (DateTime.UtcNow - requestCounter.Timestamp > _timeWindow)
            {
                requestCounter.Count = 0; 
                requestCounter.Timestamp = DateTime.UtcNow;
            }

            if (requestCounter.Count >= _requestLimit)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status429TooManyRequests,
                    Content = "Rate limit exceeded. Please try again later."
                };
                return;
            }

            requestCounter.Count++;
        }

        private class RequestCounter
        {
            public int Count { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}

