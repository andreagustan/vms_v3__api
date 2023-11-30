using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VMS.Error
{
    public class ErrorWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext httpContext, IWebHostEnvironment env)
        {
            try
            {
                await _next(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex, env);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            HttpStatusCode status;
            string message;
            var stackTrace = String.Empty;

            var exceptionType = exception.GetType();
            if (exceptionType == typeof(BadRequestException))
            {
                message = exception.Message;
                status = HttpStatusCode.BadRequest;
            }
            else if (exceptionType == typeof(NotFoundException))
            {
                message = exception.Message;
                status = HttpStatusCode.NotFound;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = env.EnvironmentName.ToLower().Contains("development") ? exception.Message : status.ToString();
            }
            if (env.EnvironmentName.ToLower().Contains("development"))
                stackTrace = exception.StackTrace;

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = (int)status,
                error = true,
                detail = message,
                message = status.ToString(),
                data = ""
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            string connectionString = Settings.AppSettingValue("ConnectionStrings", "DefaultConnection");            

            context.Request.RouteValues.TryGetValue("controller", out var moduleNya);
                        
            return context.Response.WriteAsync(result);
        }
    }
}
