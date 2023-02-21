using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using FluentValidation;

using Holefeeder.Api.Extensions;
using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.Api.ErrorHandling;

[ExcludeFromCodeCoverage]
public static class CustomErrorHandler
{
    public static WebApplication UseCustomErrors(this WebApplication app, IHostEnvironment environment)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(environment.IsDevelopment() ? WriteDevelopmentResponse : WriteProductionResponse);
        });

        return app;
    }

    private static Task WriteDevelopmentResponse(HttpContext httpContext)
    {
        return WriteResponse(httpContext, true);
    }

    private static Task WriteProductionResponse(HttpContext httpContext)
    {
        return WriteResponse(httpContext, false);
    }

    private static async Task WriteResponse(HttpContext httpContext, bool includeDetails)
    {
        var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        var ex = exceptionDetails?.Error;

        if (ex != null)
        {
            httpContext.Response.ContentType = "application/problem+json";

            var problem = ex switch
            {
                ValidationException validationException => CreateValidationProblemDetails(validationException
                    .ToDictionary()),
                NotFoundException notFoundException => CreateProblemDetails(StatusCodes.Status404NotFound,
                    $"A domain error occured: {notFoundException.Context}", notFoundException.Message),
                DomainException domainException => CreateProblemDetails(StatusCodes.Status400BadRequest,
                    $"A domain error occured: {domainException.Context}", domainException.Message),
                _ => CreateProblemDetails(httpContext.Response.StatusCode,
                    includeDetails ? "An error occured: " + ex.Message : "An error occured",
                    includeDetails ? ex.ToString() : null)
            };

            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            await JsonSerializer.SerializeAsync<object>(httpContext.Response.Body, problem);
        }
    }

    private static ProblemDetails CreateProblemDetails(int statusCode, string title, string? details)
    {
        return new ProblemDetails { Status = statusCode, Title = title, Detail = details };
    }

    private static ProblemDetails CreateValidationProblemDetails(IDictionary<string, string[]> errors)
    {
        return new ValidationProblemDetails(errors) { Status = StatusCodes.Status422UnprocessableEntity };
    }
}
