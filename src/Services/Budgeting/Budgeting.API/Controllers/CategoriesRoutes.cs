using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Categories.Queries;
using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Budgeting.API.Controllers;

public static class CategoriesRoutes
{
    public static WebApplication AddCategoriesRoutes(this WebApplication app)
    {
        const string routePrefix = "api/v2/categories";

        app.MapGet($"{routePrefix}", GetCategories)
            .WithName(nameof(GetCategories))
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces<CategoryViewModel[]>()
            .AddOptions();

        return app;
    }

    private static async Task<IResult> GetCategories(IMediator mediator, HttpResponse response)
    {
        var (total, enumerable) = await mediator.Send(new GetCategories.Request());
        response.Headers.Add("X-Total-Count", $"{total}");
        return Results.Ok(enumerable);
    }
    
    private static RouteHandlerBuilder AddOptions(this RouteHandlerBuilder builder) =>
        builder
            .WithTags("Categories")
            .RequireAuthorization()
            .WithGroupName("v2");
}
