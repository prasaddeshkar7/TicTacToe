using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TicTacToe.Core.Exceptions;

namespace TicTacToe.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
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
        _logger.LogError(exception, exception.Message);

        var problem = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {
            case GameNotFoundException:
                problem.Title = "Game not found";
                problem.Detail = exception.Message;
                problem.Status = StatusCodes.Status404NotFound;
                break;

            case InvalidMoveException:
                problem.Title = "Invalid move";
                problem.Detail = exception.Message;
                problem.Status = StatusCodes.Status409Conflict;
                break;

            case ArgumentException:
                problem.Title = "Bad request";
                problem.Detail = exception.Message;
                problem.Status = StatusCodes.Status400BadRequest;
                break;

            default:
                problem.Title = "Internal Server Error";
                problem.Detail = "An unexpected error occurred.";
                problem.Status = StatusCodes.Status500InternalServerError;
                break;
        }

        httpContext.Response.StatusCode = problem.Status!.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}