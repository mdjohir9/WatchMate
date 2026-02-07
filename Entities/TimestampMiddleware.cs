namespace WatchMate_API.Entities
{
    public class TimestampMiddleware
    {
        private readonly RequestDelegate _next;

        public TimestampMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add a timestamp query parameter if it's not already present
            var currentUrl = context.Request.Path.Value;
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); // Unique timestamp format (without milliseconds)

            // Append the timestamp as a query parameter (e.g., /api/data?t=20250220123045)
            if (!currentUrl.Contains("?"))
            {
                context.Request.Path = currentUrl + "?t=" + timestamp;
            }
            else
            {
                context.Request.Path = currentUrl + "&t=" + timestamp;
            }

            await _next(context); // Continue processing the request
        }


    }
}
