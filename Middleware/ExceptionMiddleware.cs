using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
        {
            this._next = next;
            this._logger = logger;
            this._environment = environment;
        } 
        public async Task InvokeAsync(HttpContext content)
        {
            try
            {
                await _next(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                content.Response.ContentType = "application/json";
                content.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                var response = _environment.IsDevelopment() ? new ApiException(content.Response.StatusCode, ex.Message, ex.StackTrace!.ToString())
                : new ApiException(content.Response.StatusCode, "Internal Server Error");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(response,options);
                await content.Response.WriteAsync(json);
            }
        }
    }
}