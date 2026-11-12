using System.Text.Json;

using Holefeeder.Service.Cashflows.Core.ListCashflows;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Cashflows.Api.ListCashflows;

internal static class ListCashflowsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapListCashflows(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/cashflows/manage", HandlerAsync)
            .Produces<IEnumerable<CashflowResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Cashflows")
            .WithName("ListCashflows")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        UserContextAccessor userContextAccessor,
        ListCashflowsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var cashflowsResult = await useCase.HandleAsync(new ListCashflowsRequest(userContextAccessor.Current), cancellationToken);
        if (cashflowsResult.IsFailure)
        {
            return ApiResult.ToProblem(cashflowsResult.Error);
        }

        return Results.Ok(cashflowsResult.Value.Select(cashflow => new CashflowResponse(
            cashflow.Id.Value,
            cashflow.Title.Title,
            cashflow.Description,
            cashflow.CategoryId.Value,
            cashflow.CategoryName,
            cashflow.CategoryColor,
            JsonNamingPolicy.CamelCase.ConvertName(cashflow.Schedule.CategoryType.ToString()),
            cashflow.AccountName,
            cashflow.Schedule.Amount,
            JsonNamingPolicy.CamelCase.ConvertName(cashflow.Schedule.IntervalType.Name),
            cashflow.Schedule.Frequency,
            cashflow.Schedule.EffectiveDate,
            cashflow.Schedule.NextDate(),
            cashflow.Tags)));
    }
}
