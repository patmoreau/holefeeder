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

        var cashflows = await context.Cashflows
            .Where(c => c.UserId == userContext.Id && account.Id == c.AccountId && !c.Inactive)
            .Include(c => c.Category)
            .Include(c => c.Transactions)
            .ToListAsync(cancellationToken);

        var dateInterval = DateInterval.Create(DateOnly.FromDateTime(DateTime.Now), 0, userContext.Settings.EffectiveDate, userContext.Settings.IntervalType, userContext.Settings.Frequency);

        return AccountMapper.MapToAccountViewModel(account, dateInterval.End, cashflows);
    }

    public record Request(AccountId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => AccountId.Create(x)).NotNull().NotEqual(AccountId.Empty);
    }
}
