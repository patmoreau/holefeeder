using System.Globalization;
using System.Reflection;

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.Filters;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Accounts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetUpcoming : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/cashflows/get-upcoming",
                async (Request request, IUserContext userContext, BudgetingContext context, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    ctx.Response.Headers.Append("X-Total-Count", $"{result.Total}");
                    return Results.Ok(result.Items);
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<IEnumerable<UpcomingViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetUpcoming))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<QueryResult<UpcomingViewModel>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var cashflows = await context.Cashflows
            .Where(c => c.UserId == userContext.Id &&
                        (request.AccountId == AccountId.Empty || c.AccountId == request.AccountId))
            .Include(c => c.Account)
            .Include(c => c.Category)
            .Include(c => c.Transactions)
            .ToListAsync(cancellationToken);

        var results = cashflows
            .SelectMany(x => x.GetUpcoming(request.To)
                .Select(d => new UpcomingViewModel
                {
                    Id = x.Id,
                    Date = d,
                    Amount = x.Amount,
                    Description = x.Description,
                    Tags = [..x.Tags],
                    Category = new CategoryInfoViewModel(x.Category!.Id, x.Category.Name, x.Category.Type,
                        x.Category.Color),
                    Account = new AccountInfoViewModel(x.Account!.Id, x.Account.Name)
                }))
            .Where(x => x.Date <= request.To)
            .OrderBy(x => x.Date)
            .ToList();

        return new QueryResult<UpcomingViewModel>(results.Count, results);
    }

    internal record Request(DateOnly From, DateOnly To, AccountId AccountId)
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string fromKey = "from";
            const string toKey = "to";
            const string accountIdKey = "accountId";

            var hasFrom = DateOnly.TryParse(context.Request.Query[fromKey], CultureInfo.InvariantCulture, out var from);
            var hasTo = DateOnly.TryParse(context.Request.Query[toKey], CultureInfo.InvariantCulture, out var to);
            var hasAccountId = AccountId.TryParse(context.Request.Query[accountIdKey], CultureInfo.InvariantCulture,
                out var accountId);

            Request result = new(hasFrom ? from : DateOnly.MinValue, hasTo ? to : DateOnly.MaxValue,
                hasAccountId ? accountId! : AccountId.Empty);

            return ValueTask.FromResult<Request?>(result);
        }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.From).NotEmpty();
            RuleFor(command => command.To)
                .NotEmpty()
                .GreaterThanOrEqualTo(command => command.From)
                .WithMessage($"{nameof(Request.To)} must be greater or equal to {nameof(Request.To)}.");
        }
    }
}
