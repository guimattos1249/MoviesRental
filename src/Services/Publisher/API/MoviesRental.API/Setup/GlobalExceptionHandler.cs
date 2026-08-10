using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using MoviesRental.Core.DomainObjects;

namespace MoviesRental.API.Setup;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string errorMessage) = exception switch
        {
            ArgumentException argumentException => (500, argumentException.Message),
            DomainException domainException => (500, domainException.Message),
            SqlException sqlException => (500, sqlException.Message),
            ValidationException validationException => (500, validationException.Message),
            _ => (500, "Sometihing went wrong")
        };

        _logger.LogError(exception, exception.Message);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorMessage, cancellationToken);
        return true;

    }
}
