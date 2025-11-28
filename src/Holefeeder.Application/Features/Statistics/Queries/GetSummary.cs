using System.Globalization;
using System.Reflection;

using DrifterApps.Seeds.Application.EndpointFilters;

using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Statistics.Queries;

public class GetSummary : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/summary/statistics",
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var results = await Handle(request, userContext, context, cancellationToken);
                    return Results.Ok(results);
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<SummaryDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Statistics))
            .WithName(nameof(GetSummary))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<SummaryDto> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var (intervalType, frequency) = DateIntervalType.GetIntervalTypeFromRange(request.From, request.To);
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
            .GroupBy(x => intervalType.Interval(request.From, x.Date, frequency))
            .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
            .ToDictionary();
        var expenses = results
            .Where(x => x.Type == CategoryType.Expense)
            .GroupBy(x => intervalType.Interval(request.From, x.Date, frequency))
            .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
            .ToDictionary();

        return new SummaryDto(
            new SummaryValue(Period(gains, intervalType.AddIteration(request.From, -frequency)),
                Period(expenses, intervalType.AddIteration(request.From, -frequency))),
            new SummaryValue(Period(gains, request.From), Period(expenses, request.From)),
            new SummaryValue(Average(gains), Average(expenses)));
    }

    private static decimal Period(Dictionary<(DateOnly From, DateOnly To), decimal> dto, DateOnly asOf) =>
        dto.FirstOrDefault(x => x.Key.From == asOf).Value;

    private static decimal Average(Dictionary<(DateOnly From, DateOnly To), decimal> dto) =>
        dto.Count > 0 ? Math.Round(dto.Sum(x => x.Value) / dto.Count, 2) : 0;

    internal record Request(DateOnly From, DateOnly To)
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string fromKey = "from";
            const string toKey = "to";

            var hasFromDate = DateOnly.TryParse(context.Request.Query[fromKey], CultureInfo.InvariantCulture, out var from);
            var hasToDate = DateOnly.TryParse(context.Request.Query[toKey], CultureInfo.InvariantCulture, out var to);

            Request result = new(hasFromDate ? from : default, hasToDate ? to : default);

            return ValueTask.FromResult<Request?>(result);
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.From).NotEmpty();
            RuleFor(command => command.To).NotEmpty();
        }
    }
}
