using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace DrifterApps.Holefeeder.Budgeting.API.Middlewares;

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

            int statusCode;
            string title;
            string? details;
            switch (ex)
            {
                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = validationException.Source ?? nameof(ValidationException);
                    details = validationException.Message;
                    break;
                case HolefeederDomainException domainException:
                    statusCode = StatusCodes.Status400BadRequest;
                    title = $"A domain error occured: {domainException.Context}";
                    details = domainException.Message;
                    break;
                case NotFoundDomainException notFoundDomainException:
                    statusCode = StatusCodes.Status404NotFound;
                    title = $"A domain error occured: {notFoundDomainException.Context}";
                    details = notFoundDomainException.Message;
                    break;
                default:
                    statusCode = httpContext.Response.StatusCode;
                    title = includeDetails ? "An error occured: " + ex.Message : "An error occured";
                    details = includeDetails ? ex.ToString() : null;
                    break;
            }

            var problem = new ProblemDetails {Status = statusCode, Title = title, Detail = details};

            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problem.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = statusCode;
            var stream = httpContext.Response.Body;
            await JsonSerializer.SerializeAsync(stream, problem);
        }
    }
}
