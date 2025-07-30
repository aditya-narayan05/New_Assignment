//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.IO;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace backendNew.Logging
//{
//    // Enhanced middleware for comprehensive request/response/exception logging
//    public class ComprehensiveLoggingMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly string _logDirectory;
//        private readonly string _requestLogFile;
//        private readonly string _exceptionLogFile;

//        public ComprehensiveLoggingMiddleware(RequestDelegate next)
//        {
//            _next = next;
//            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
//            _requestLogFile = Path.Combine(_logDirectory, "requests.log");
//            _exceptionLogFile = Path.Combine(_logDirectory, "exceptions.log");

//            // Ensure log directory exists
//            if (!Directory.Exists(_logDirectory))
//            {
//                Directory.CreateDirectory(_logDirectory);
//            }
//        }

//        // Overloaded constructor for custom log directory
//        public ComprehensiveLoggingMiddleware(RequestDelegate next, string logDirectory)
//        {
//            _next = next;
//            _logDirectory = logDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");
//            _requestLogFile = Path.Combine(_logDirectory, "requests.log");
//            _exceptionLogFile = Path.Combine(_logDirectory, "exceptions.log");

//            // Ensure log directory exists
//            if (!Directory.Exists(_logDirectory))
//            {
//                Directory.CreateDirectory(_logDirectory);
//            }
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            var startTime = DateTime.UtcNow;
//            Exception caughtException = null;

//            // Capture request body
//            var requestBody = await CaptureRequestBodyAsync(context);

//            // Store original response body stream
//            var originalResponseBodyStream = context.Response.Body;

//            try
//            {
//                using var responseBodyStream = new MemoryStream();
//                context.Response.Body = responseBodyStream;

//                await _next(context);

//                // Capture response body
//                var responseBody = await CaptureResponseBodyAsync(responseBodyStream);

//                // Copy response back to original stream
//                responseBodyStream.Seek(0, SeekOrigin.Begin);
//                await responseBodyStream.CopyToAsync(originalResponseBodyStream);

//                // Log successful request
//                await LogRequestAsync(context, startTime, requestBody, responseBody, null);
//            }
//            catch (Exception ex)
//            {
//                caughtException = ex;

//                // Log exception
//                await LogExceptionAsync(context, startTime, requestBody, ex);

//                // Handle exception response
//                context.Response.Body = originalResponseBodyStream;
//                await HandleExceptionResponseAsync(context, ex);
//            }
//        }

//        private async Task<string> CaptureRequestBodyAsync(HttpContext context)
//        {
//            try
//            {
//                if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
//                {
//                    context.Request.EnableBuffering();
//                    var buffer = new byte[context.Request.ContentLength.Value];
//                    await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
//                    context.Request.Body.Position = 0;
//                    return Encoding.UTF8.GetString(buffer);
//                }
//            }
//            catch (Exception ex)
//            {
//                return $"Error reading request body: {ex.Message}";
//            }
//            return string.Empty;
//        }

//        private async Task<string> CaptureResponseBodyAsync(MemoryStream responseBodyStream)
//        {
//            try
//            {
//                responseBodyStream.Seek(0, SeekOrigin.Begin);
//                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
//                return responseBody;
//            }
//            catch (Exception ex)
//            {
//                return $"Error reading response body: {ex.Message}";
//            }
//        }

//        private async Task LogRequestAsync(HttpContext context, DateTime startTime, string requestBody, string responseBody, Exception exception)
//        {
//            var duration = DateTime.UtcNow - startTime;
//            var isError = exception != null || context.Response.StatusCode >= 400;

//            var logEntry = new
//            {
//                Timestamp = startTime,
//                RequestId = context.TraceIdentifier,
//                Request = new
//                {
//                    Method = context.Request.Method,
//                    Path = context.Request.Path.ToString(),
//                    QueryString = context.Request.QueryString.ToString(),
//                    Headers = GetHeaders(context.Request.Headers),
//                    Body = requestBody,
//                    ContentType = context.Request.ContentType,
//                    ContentLength = context.Request.ContentLength
//                },
//                Response = new
//                {
//                    StatusCode = context.Response.StatusCode,
//                    Headers = GetHeaders(context.Response.Headers),
//                    Body = responseBody,
//                    ContentType = context.Response.ContentType
//                },
//                Performance = new
//                {
//                    Duration = duration.TotalMilliseconds,
//                    DurationFormatted = $"{duration.TotalMilliseconds:F2}ms"
//                },
//                Client = new
//                {
//                    IPAddress = context.Connection.RemoteIpAddress?.ToString(),
//                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
//                    Referrer = context.Request.Headers["Referer"].ToString()
//                },
//                IsError = isError,
//                Exception = exception != null ? FormatException(exception) : null
//            };

//            var logText = $"{JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true })}{Environment.NewLine}{"=".PadRight(100, '=')}{Environment.NewLine}";

//            try
//            {
//                await File.AppendAllTextAsync(_requestLogFile, logText);
//            }
//            catch (Exception logEx)
//            {
//                Console.WriteLine($"Failed to write request log: {logEx.Message}");
//            }
//        }

//        private async Task LogExceptionAsync(HttpContext context, DateTime startTime, string requestBody, Exception exception)
//        {
//            var logEntry = new
//            {
//                Timestamp = startTime,
//                RequestId = context.TraceIdentifier,
//                Request = new
//                {
//                    Method = context.Request.Method,
//                    Path = context.Request.Path.ToString(),
//                    QueryString = context.Request.QueryString.ToString(),
//                    Body = requestBody,
//                    Headers = GetHeaders(context.Request.Headers)
//                },
//                Client = new
//                {
//                    IPAddress = context.Connection.RemoteIpAddress?.ToString(),
//                    UserAgent = context.Request.Headers["User-Agent"].ToString()
//                },
//                Exception = FormatException(exception)
//            };

//            var logText = $"{JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true })}{Environment.NewLine}{"=".PadRight(100, '=')}{Environment.NewLine}";

//            try
//            {
//                await File.AppendAllTextAsync(_exceptionLogFile, logText);
//            }
//            catch (Exception logEx)
//            {
//                Console.WriteLine($"Failed to write exception log: {logEx.Message}");
//            }
//        }

//        private async Task HandleExceptionResponseAsync(HttpContext context, Exception exception)
//        {
//            if (!context.Response.HasStarted)
//            {
//                context.Response.ContentType = "application/json";
//                context.Response.StatusCode = GetStatusCode(exception);

//                var response = new
//                {
//                    error = new
//                    {
//                        message = "An error occurred while processing your request.",
//                        requestId = context.TraceIdentifier,
//                        timestamp = DateTime.UtcNow,
//                        type = exception.GetType().Name
//                    }
//                };

//                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
//            }
//        }

//        private static object GetHeaders(IHeaderDictionary headers)
//        {
//            var headerDict = new Dictionary<string, string>();
//            foreach (var header in headers)
//            {
//                // Skip sensitive headers
//                if (IsSensitiveHeader(header.Key))
//                    headerDict[header.Key] = "[REDACTED]";
//                else
//                    headerDict[header.Key] = header.Value.ToString();
//            }
//            return headerDict;
//        }

//        private static bool IsSensitiveHeader(string headerName)
//        {
//            var sensitiveHeaders = new[] { "authorization", "cookie", "x-api-key", "x-auth-token" };
//            return sensitiveHeaders.Contains(headerName.ToLower());
//        }

//        private static object FormatException(Exception exception)
//        {
//            return new
//            {
//                Type = exception.GetType().Name,
//                Message = exception.Message,
//                StackTrace = exception.StackTrace,
//                Source = exception.Source,
//                Data = exception.Data.Count > 0 ? exception.Data : null,
//                InnerException = exception.InnerException != null ? FormatException(exception.InnerException) : null
//            };
//        }

//        private static int GetStatusCode(Exception exception)
//        {
//            return exception switch
//            {
//                ArgumentException => 400,
//                UnauthorizedAccessException => 401,
//                NotImplementedException => 501,
//                TimeoutException => 408,
//                _ => 500
//            };
//        }
//    }

//    // Global exception filter to catch ALL exceptions (even handled ones)
//    public class GlobalExceptionFilter : IExceptionFilter
//    {
//        private readonly string _handledExceptionLogFile;

//        public GlobalExceptionFilter()
//        {
//            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
//            _handledExceptionLogFile = Path.Combine(logDir, "handled-exceptions.log");

//            if (!Directory.Exists(logDir))
//            {
//                Directory.CreateDirectory(logDir);
//            }
//        }

//        // Overloaded constructor for custom log directory
//        public GlobalExceptionFilter(string logDirectory)
//        {
//            var logDir = logDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "Logs");
//            _handledExceptionLogFile = Path.Combine(logDir, "handled-exceptions.log");

//            if (!Directory.Exists(logDir))
//            {
//                Directory.CreateDirectory(logDir);
//            }
//        }

//        public void OnException(ExceptionContext context)
//        {
//            // Log the exception (this catches even exceptions that will be handled)
//            Task.Run(() => LogHandledException(context));
//        }

//        private async Task LogHandledException(ExceptionContext context)
//        {
//            var logEntry = new
//            {
//                Timestamp = DateTime.UtcNow,
//                RequestId = context.HttpContext.TraceIdentifier,
//                ActionName = context.ActionDescriptor.DisplayName,
//                ControllerName = context.ActionDescriptor.RouteValues["controller"],
//                ActionMethod = context.ActionDescriptor.RouteValues["action"],
//                Request = new
//                {
//                    Method = context.HttpContext.Request.Method,
//                    Path = context.HttpContext.Request.Path.ToString(),
//                    QueryString = context.HttpContext.Request.QueryString.ToString()
//                },
//                Exception = new
//                {
//                    Type = context.Exception.GetType().Name,
//                    Message = context.Exception.Message,
//                    StackTrace = context.Exception.StackTrace,
//                    InnerException = context.Exception.InnerException?.Message
//                },
//                WasHandled = context.ExceptionHandled,
//                Client = new
//                {
//                    IPAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
//                    UserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString()
//                }
//            };

//            var logText = $"{JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true })}{Environment.NewLine}{"=".PadRight(100, '=')}{Environment.NewLine}";

//            try
//            {
//                await File.AppendAllTextAsync(_handledExceptionLogFile, logText);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Failed to write handled exception log: {ex.Message}");
//            }
//        }
//    }

//    // Extension methods for easy registration
//    public static class LoggingExtensions
//    {
//        public static IApplicationBuilder UseComprehensiveLogging(this IApplicationBuilder builder)
//        {
//            return builder.UseMiddleware<ComprehensiveLoggingMiddleware>();
//        }

//        public static IApplicationBuilder UseComprehensiveLogging(this IApplicationBuilder builder, string logDirectory)
//        {
//            return builder.UseMiddleware<ComprehensiveLoggingMiddleware>(logDirectory);
//        }

//        public static IServiceCollection AddComprehensiveLogging(this IServiceCollection services)
//        {
//            services.AddScoped<GlobalExceptionFilter>();
//            return services;
//        }

//        public static IServiceCollection AddComprehensiveLogging(this IServiceCollection services, string logDirectory)
//        {
//            services.AddScoped(_ => new GlobalExceptionFilter(logDirectory));
//            return services;
//        }
//    }
//}