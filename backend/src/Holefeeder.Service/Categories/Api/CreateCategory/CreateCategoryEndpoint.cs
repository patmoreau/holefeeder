using Holefeeder.Service.Categories.Core.CreateCategory;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Categories.Api.CreateCategory;

internal static class CreateCategoryEndpoint
{
    private const string WriteUserPolicy = "WriteUser";

    public static IEndpointRouteBuilder MapCreateCategory(this IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/categories", HandlerAsync)
            .Produces<CreateCategoryResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .WithTags("Categories")
            .WithName("CreateCategory")
            .RequireAuthorization(WriteUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] CreateCategoryRequestRaw rawRequest,
        UserContextAccessor userContextAccessor,
        CreateCategoryUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = CreateCategoryRequestRaw.ToCreateCategoryRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }

        var idResult = await useCase.HandleAsync(requestResult.Value, cancellationToken);
        if (idResult.IsFailure)
        {
            return ApiResult.ToProblem(idResult.Error);
        }

        return Results.Created((string?)null, new CreateCategoryResponse(idResult.Value.Value));
    }
}
