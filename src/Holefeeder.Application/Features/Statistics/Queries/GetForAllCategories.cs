using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Statistics.Queries;

public class GetForAllCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/categories/statistics",
                async (IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var results = await Handle(userContext, context, cancellationToken);
                    return Results.Ok(results);
                })
            .Produces<IEnumerable<StatisticsDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Statistics))
            .WithName(nameof(GetForAllCategories))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<IEnumerable<StatisticsDto>> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var query = from category in context.Categories
                    join transaction in context.Transactions on category.Id equals transaction.CategoryId
                    where category.UserId == userContext.Id && !category.System
                    group transaction by new
                    {
                        category.Id,
                        category.Name,
                        category.Type,
                        category.Color,
                        transaction.Date.Year,
                        transaction.Date.Month
                    }
            into groupedTransactions
                    select new
                    {
                        groupedTransactions.Key.Id,
                        groupedTransactions.Key.Name,
                        groupedTransactions.Key.Type,
                        groupedTransactions.Key.Color,
                        groupedTransactions.Key.Year,
                        groupedTransactions.Key.Month,
                        TotalAmount = groupedTransactions.Sum(t => t.Amount)
                    }
            into summarizedTransactions
                    group summarizedTransactions by new
                    {
                        summarizedTransactions.Id,
                        summarizedTransactions.Name,
                        summarizedTransactions.Type,
                        summarizedTransactions.Color,
                        summarizedTransactions.Year
                    }
            into groupedSummaries
                    select new
                    {
                        groupedSummaries.Key.Id,
                        groupedSummaries.Key.Name,
                        groupedSummaries.Key.Type,
                        groupedSummaries.Key.Color,
                        groupedSummaries.Key.Year,
                        TotalAmountByYear = groupedSummaries.Sum(s => s.TotalAmount),
                        MonthlyTotals = groupedSummaries.Select(s => new { s.Month, TotalAmountByMonth = s.TotalAmount })
                    };

        var results = await query.ToListAsync(cancellationToken: cancellationToken);

        var dto = results
            .Where(x => x.Type.IsExpense)
            .GroupBy(x => new { x.Id, x.Name, x.Color },
                arg => new YearStatisticsDto(arg.Year, arg.TotalAmountByYear,
                    arg.MonthlyTotals.Select(x => new MonthStatisticsDto(x.Month, x.TotalAmountByMonth)).ToList()))
            .OrderBy(x => x.Key.Name);

        return dto.Select(grouping => new StatisticsDto(grouping.Key.Id, grouping.Key.Name, grouping.Key.Color,
            Math.Round(grouping.Sum(x => x.Total) / grouping.Sum(x => x.Months.Count()), 2), grouping));
    }
}
