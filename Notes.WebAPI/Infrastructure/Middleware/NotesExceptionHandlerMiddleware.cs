using System.Net;
using System.Text.Json;
using Notes.Common.Exceptions;

namespace Notes.WebAPI.Infrastructure.Middleware
{
    public class NotesExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly ILogger<NotesExceptionHandlerMiddleware> _logger;

        public NotesExceptionHandlerMiddleware(RequestDelegate request, ILogger<NotesExceptionHandlerMiddleware> logger)
        {
            _request = request;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _request(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionType = exception switch
            {
                ServiceException serviceException => serviceException.Type,
                _ => ExceptionEnum.Undefined
            };

            switch (exceptionType)
            {
                case ExceptionEnum.BadRequest:
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Reference = exception.GetType().Name,
                        Message = exception.Message,
                        Data = exception.Data
                    }));
                    _logger.LogWarning($"BadRequest - Message: {exception.Message} " +
                                       $"Data: {JsonSerializer.Serialize(exception.Data)}");
                    break;
                case ExceptionEnum.Forbidden:
                    context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                    break;
                case ExceptionEnum.NotFound:
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    _logger.LogWarning($"NotFound - Message: {exception.Message}");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Reference = exception.GetType().Name,
                        Message = exception.Message,
                        Data = exception.Data
                    }));
                    break;
                default:
                    _logger.LogCritical($"Failed to handle exception in code. {exception.Message}");
                    throw exception;
            }
        }
    }
}