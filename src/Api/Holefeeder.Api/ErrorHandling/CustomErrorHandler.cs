using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using DrifterApps.Seeds.Domain;

using FluentValidation;

using Holefeeder.Api.Extensions;
using Holefeeder.Application.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.Api.ErrorHandling;

[ExcludeFromCodeCoverage]
public static class CustomErrorHandler
{
    public static void UseCustomErrors(this WebApplication app, IHostEnvironment environment) =>
        app.UseExceptionHandler(builder =>
        {
            builder.Run(environment.IsDevelopment() ? WriteDevelopmentResponse : WriteProductionResponse);
        });

    private static Task WriteDevelopmentResponse(HttpContext httpContext) => WriteResponse(httpContext, true);

    private static Task WriteProductionResponse(HttpContext httpContext) => WriteResponse(httpContext, false);

    private static Task WriteResponse(HttpContext httpContext, bool includeDetails)
    {
        var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
        var ex = exceptionDetails?.Error;

        if (ex == null)
        {
            return Task.CompletedTask;
        }

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
        return JsonSerializer.SerializeAsync<object>(httpContext.Response.Body, problem);

    }

    private static ProblemDetails CreateProblemDetails(int statusCode, string title, string? details) =>
        new() { Status = statusCode, Title = title, Detail = details };

    private static ValidationProblemDetails CreateValidationProblemDetails(IDictionary<string, string[]> errors) =>
        new(errors) { Status = StatusCodes.Status422UnprocessableEntity };
}
