namespace backendNew.NewMiddleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiLoggingMiddleware> _logger;

        public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log Request
            context.Request.EnableBuffering(); //allows the request body to be read multiple times
            var requestBodyStream = new StreamReader(context.Request.Body);
            string requestBodyText = await requestBodyStream.ReadToEndAsync(); //reads body into a string so it can be logged
            context.Request.Body.Position = 0; //reset stream so other middleware can read it

            _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} | Body: {requestBodyText}");

            // Replace response stream
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream(); 
            context.Response.Body = responseBody; //temporary replaces it with memory stream

            await _next(context); //reads the response and logs it

            // Log Response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"Response: {context.Response.StatusCode} | Body: {responseText}");

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
