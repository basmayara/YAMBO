using System.Net;
using System.Text.Json;

namespace YamboAPI.Middleware
{
    // ✅ Global Exception Middleware — replaces try-catch in every controller
    // Instead of wrapping each action in try-catch, we catch ALL unhandled exceptions here
    // This follows the "Single Responsibility" principle: controllers handle logic, middleware handles errors
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        // ✅ Dependency Injection: ILogger is injected by the DI container (Transient by default for loggers)
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // ✅ Function chaining: passes request to the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // 🔴 Log the full error server-side (never expose stack trace to client)
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);

                // ✅ XSS Protection: we return JSON, not HTML — so no script injection possible in error responses
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    success = false,
                    message = "An internal error occurred. Please try again later."
                    // ✅ Never expose ex.Message or stack trace to client — security best practice
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}