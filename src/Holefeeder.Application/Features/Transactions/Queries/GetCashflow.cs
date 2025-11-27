using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.Filters;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/cashflows/{id:guid}",
                async (Guid id, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(new Request(CashflowId.Create(id)), userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.Ok(result.Value)
                    };
                })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<CashflowInfoViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetCashflow))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<CashflowInfoViewModel>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var result = await context.Cashflows
            .Include(x => x.Account).Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
        return result is null
            ? CashflowErrors.NotFound(request.Id)
            : CashflowMapper.MapToDto(result);
    }

    public record Request(CashflowId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => CashflowId.Create(x)).NotEqual(CashflowId.Empty);
    }
}
