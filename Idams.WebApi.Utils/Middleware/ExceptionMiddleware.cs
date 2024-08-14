using Idams.Core.Model.Entities.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Idams.WebApi.Utils.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }            
            catch (Exception ex)
            {
#if DEBUG
                var error = GenerateMessage("", ex);
                Debugger.Break();
#endif
                _logger.LogError(ex, "ERROR processing request");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private object GenerateMessage(string msg, Exception ex)
        {
            var message = msg + "|" + ex.Message;
            if (ex.InnerException != null)
                return GenerateMessage(message, ex.InnerException);
            return message;
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                _ => (int)HttpStatusCode.InternalServerError
            };

            var status = exception switch
            {
                _ => "Internal Server Error"
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(new ErrorDetails()
            {
                code = context.Response.StatusCode,
                status = status,
                message = exception.Message
            }.ToString());
        }
    }
}
