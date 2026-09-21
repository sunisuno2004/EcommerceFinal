using ECommerce.Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                ValidationException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Validation Error"
                    ),

                UnauthorizedException =>
                    (
                        StatusCodes.Status401Unauthorized,
                        "Unauthorized"
                    ),

                NotFoundException =>
                    (
                        StatusCodes.Status404NotFound,
                        "Resource Not Found"
                    ),

                ConflictException =>
                    (
                        StatusCodes.Status409Conflict,
                        "Conflict"
                    ),

                _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "Internal Server Error"
                    )
            };

            if (statusCode ==
                StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "An unhandled exception occurred.");
            }
            else
            {
                _logger.LogWarning(
                    exception,
                    "Request failed with status code {StatusCode}.",
                    statusCode);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,

                Detail =
                    statusCode ==
                    StatusCodes.Status500InternalServerError
                        ? "An unexpected error occurred."
                        : exception.Message,

                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
    problemDetails);

            return true;
        }
    }
}