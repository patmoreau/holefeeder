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

public class GetTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/transactions/{id:guid}",
                async (Guid id, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(new Request(TransactionId.Create(id)), userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.Ok(result.Value)
                    };
                })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .Produces<TransactionInfoViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetTransaction))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<TransactionInfoViewModel>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var transaction = await context.Transactions
            .Include(x => x.Account)
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
        return transaction is null
            ? TransactionErrors.NotFound(request.Id)
            : TransactionMapper.MapToDto(transaction);
    }

    public record Request(TransactionId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(x => TransactionId.Create(x)).NotEqual(TransactionId.Empty);
    }
}
