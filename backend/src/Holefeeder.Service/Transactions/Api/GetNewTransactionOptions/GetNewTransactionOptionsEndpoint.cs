using System.Text.Json;

using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Transactions.Core.GetNewTransactionOptions;

namespace Holefeeder.Service.Transactions.Api.GetNewTransactionOptions;

internal static class GetNewTransactionOptionsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapGetNewTransactionOptions(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/transactions/new-transaction-options", HandlerAsync)
            .Produces<NewTransactionOptionsResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Transactions")
            .WithName("TransactionsGetNewTransactionOptions")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        GetNewTransactionOptionsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var optionsResult = await useCase.HandleAsync(new GetNewTransactionOptionsRequest(userContextAccessor.Current), cancellationToken);
        if (optionsResult.IsFailure)
        {
            return ApiResult.ToProblem(optionsResult.Error);
        }
        var options = optionsResult.Value;

        return Results.Ok(new NewTransactionOptionsResponse(
            [.. options.Accounts.Select(account => new AccountOptionResponse(account.Id.Value, account.Name))],
            [
                .. options.Categories.Select(category => new CategoryOptionResponse(
                    category.Id.Value, category.Name, category.Color, JsonNamingPolicy.CamelCase.ConvertName(category.Type.ToString())))
            ]));
    }
}
