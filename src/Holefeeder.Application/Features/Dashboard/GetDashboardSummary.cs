using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.UseCases.Dashboard;
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
            .Produces<SummaryResult>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Dashboard))
            .WithName(nameof(GetDashboardSummary))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<SummaryResult> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var dateInterval = DateInterval.Create(DateOnly.FromDateTime(DateTime.Now), 0, userContext.Settings.EffectiveDate, userContext.Settings.IntervalType, userContext.Settings.Frequency);
        var (intervalType, frequency) = DateIntervalType.GetIntervalTypeFromRange(dateInterval.Start, dateInterval.End);

        var query = from category in context.Categories
                    join transaction in context.Transactions on category.Id equals transaction.CategoryId
                    where category.UserId == userContext.Id && !category.System
                    group transaction by new
                    {
                        category.Type,
                        transaction.Date
                    }
            into groupedTransactions
                    select new SummaryData(
                        groupedTransactions.Key.Type,
                        groupedTransactions.Key.Date,
                        groupedTransactions.Sum(t => t.Amount));

        var results = await query.ToListAsync(cancellationToken);

        return SummaryCalculator.Calculate(results, dateInterval, intervalType, frequency);
    }
}
