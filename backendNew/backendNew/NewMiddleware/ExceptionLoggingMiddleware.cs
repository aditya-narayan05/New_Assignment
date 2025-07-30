namespace backendNew.NewMiddleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); //any exception its caught here if it bubbles
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled Exception: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorResponse = new { Message = "Something went wrong. Please try again later." };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
