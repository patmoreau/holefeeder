using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Queries;

public class GetAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/accounts/{id:guid}",
                async (Guid id, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(new Request(AccountId.Create(id)), userContext, context, cancellationToken);
                    return result switch
                    {
                        {IsFailure: true} => result.Error.ToProblem(),
                        _ => Results.Ok(result.Value)
                    };
                })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<AccountViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccount))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<AccountViewModel>> Handle(Request query, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var account = await context.Accounts
            .Include(e => e.Cashflows.Where(c => !c.Inactive)).ThenInclude(x => x.Category)
            .Include(e => e.Transactions).ThenInclude(e => e.Category)
            .SingleOrDefaultAsync(x => x.Id == query.Id && x.UserId == userContext.Id,
                cancellationToken);
        if (account is null)
        {
            return AccountErrors.NotFound(query.Id);
        }

        var dateInterval = DateInterval.Create(DateOnly.FromDateTime(DateTime.Now), 0, userContext.Settings.EffectiveDate, userContext.Settings.IntervalType, userContext.Settings.Frequency);

        var balance = account.CalculateBalance();
        var lastTransactionDate = account.CalculateLastTransactionDate();
        var upcomingVariation = account.CalculateUpcomingVariation(dateInterval.End);

        return new AccountViewModel(
            account.Id,
            account.Type,
            account.Name,
            account.OpenBalance,
            account.OpenDate,
            account.Transactions.Count,
            balance,
            lastTransactionDate,
            upcomingVariation,
            balance + upcomingVariation,
            account.Description,
            account.Favorite,
            account.Inactive
        );
    }

    public record Request(AccountId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => AccountId.Create(x)).NotNull().NotEqual(AccountId.Empty);
    }
}
