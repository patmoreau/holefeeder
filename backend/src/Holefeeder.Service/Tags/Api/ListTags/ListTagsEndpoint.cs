using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Tags.Core.ListTags;

namespace Holefeeder.Service.Tags.Api.ListTags;

internal static class ListTagsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapListTags(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/tags/manage", HandlerAsync)
            .Produces<IEnumerable<TagResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Tags")
            .WithName("ListTags")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        ListTagsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var tagsResult = await useCase.HandleAsync(new ListTagsRequest(userContextAccessor.Current), cancellationToken);
        if (tagsResult.IsFailure)
        {
            return ApiResult.ToProblem(tagsResult.Error);
        }

        return Results.Ok(tagsResult.Value.Select(tag => new TagResponse(tag.Tag, tag.TransactionCount, tag.ActiveCashflowCount)));
    }
}
