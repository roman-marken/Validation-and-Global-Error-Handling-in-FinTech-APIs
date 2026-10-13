using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NeoBankApi.Domain.Exceptions;

namespace NeoBankApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        _logger.LogError(exception, "Request failed. TraceId: {TraceId}", traceId);

        if (exception is AppValidationException appValidationException)
        {
            await WriteValidationProblem(httpContext, appValidationException.Errors, traceId, cancellationToken);
            return true;
        }

        if (exception is ValidationException fluentValidationException)
        {
            var errors = fluentValidationException.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

            await WriteValidationProblem(httpContext, errors, traceId, cancellationToken);
            return true;
        }

        if (exception is BaseException baseException)
        {
            var problem = new ProblemDetails
            {
                Status = (int)baseException.StatusCode,
                Title = "Business error",
                Detail = baseException.Message,
                Instance = httpContext.Request.Path
            };

            problem.Extensions["errorCode"] = baseException.ErrorCode;
            problem.Extensions["traceId"] = traceId;

            await WriteProblem(httpContext, problem, cancellationToken);
            return true;
        }

        var detail = _environment.IsProduction()
            ? "Сталася помилка сервера. Зверніться до служби підтримки та передайте traceId."
            : exception.Message;

        var serverProblem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error",
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        serverProblem.Extensions["traceId"] = traceId;

        await WriteProblem(httpContext, serverProblem, cancellationToken);
        return true;
    }

    private static async Task WriteValidationProblem(
        HttpContext httpContext,
        IDictionary<string, string[]> errors,
        string traceId,
        CancellationToken cancellationToken)
    {
        var problem = new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        await WriteProblem(httpContext, problem, cancellationToken);
    }

    private static async Task WriteProblem(
        HttpContext httpContext,
        ProblemDetails problem,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        if (problem is HttpValidationProblemDetails validationProblem)
        {
            await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
    }
}
