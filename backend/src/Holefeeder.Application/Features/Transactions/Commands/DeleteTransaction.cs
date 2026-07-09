using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Transactions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class DeleteTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("api/v2/transactions/{id:guid}",
                async (Guid id, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(new Request(TransactionId.Create(id)), userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.NoContent()
                    };
                })
            .AddEndpointFilter<ValidationFilter<Guid>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(DeleteTransaction))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<Nothing>> Handle(Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var transaction =
            await context.Transactions.SingleOrDefaultAsync(
                x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
        if (transaction is null)
        {
            return TransactionErrors.NotFound(request.Id);
        }

        context.Remove(transaction);

        return Nothing.Value;
    }

    public record Request(TransactionId Id);

    internal class Validator : AbstractValidator<Guid>
    {
        public Validator() => RuleFor(id => TransactionId.Create(id)).NotNull().NotEqual(TransactionId.Empty);
    }
}
