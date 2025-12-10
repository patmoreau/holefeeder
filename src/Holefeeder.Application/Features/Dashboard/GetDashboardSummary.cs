using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Dashboard;

public class GetDashboardSummary : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/dashboard/summary",
                async (IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var results = await Handle(userContext, context, cancellationToken);
                    return Results.Ok(results);
                })
            .Produces<SummaryDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Dashboard))
            .WithName(nameof(GetDashboardSummary))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<SummaryDto> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var dateInterval = DateInterval.Create(DateOnly.FromDateTime(DateTime.Now), 1, userContext.Settings.EffectiveDate, userContext.Settings.IntervalType, userContext.Settings.Frequency);
        var (intervalType, frequency) = DateIntervalType.GetIntervalTypeFromRange(dateInterval.Start, dateInterval.End);
        var query = from category in context.Categories
                    join transaction in context.Transactions on category.Id equals transaction.CategoryId
                    where category.UserId == userContext.Id
                    group transaction by new
                    {
                        category.Type,
                        transaction.Date
                    }
            into groupedTransactions
                    select new
                    {
                        groupedTransactions.Key.Type,
                        groupedTransactions.Key.Date,
                        TotalAmount = groupedTransactions.Sum(t => t.Amount)
                    };

        var results = await query.ToListAsync(cancellationToken);

        var gains = results
            .Where(x => x.Type == CategoryType.Gain)
            .GroupBy(x => intervalType.Interval(dateInterval.Start, x.Date, frequency))
            .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
            .ToDictionary();
        var expenses = results
            .Where(x => x.Type == CategoryType.Expense)
            .GroupBy(x => intervalType.Interval(dateInterval.Start, x.Date, frequency))
            .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
            .ToDictionary();

        return new SummaryDto(
            new SummaryValue(Period(gains, intervalType.AddIteration(dateInterval.Start, -frequency)),
                Period(expenses, intervalType.AddIteration(dateInterval.Start, -frequency))),
            new SummaryValue(Period(gains, dateInterval.Start), Period(expenses, dateInterval.Start)),
            new SummaryValue(Average(gains), Average(expenses)));
    }

    private static decimal Period(Dictionary<(DateOnly From, DateOnly To), decimal> dto, DateOnly asOf) =>
        dto.FirstOrDefault(x => x.Key.From == asOf).Value;

    private static decimal Average(Dictionary<(DateOnly From, DateOnly To), decimal> dto) =>
        dto.Count > 0 ? Math.Round(dto.Sum(x => x.Value) / dto.Count, 2) : 0;
}
