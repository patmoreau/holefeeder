using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
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
        app.MapGet("api/v2/transactions/{id}",
                async (TransactionId id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new Request(id), cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.Ok(result.Value)
                    };
                })
            .Produces<TransactionInfoViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetTransaction))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request(TransactionId Id) : IRequest<Result<TransactionInfoViewModel>>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id).NotEqual(TransactionId.Empty);
    }

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, Result<TransactionInfoViewModel>>
    {
        public async Task<Result<TransactionInfoViewModel>> Handle(Request query,
            CancellationToken cancellationToken)
        {
            var transaction = await context.Transactions
                .Include(x => x.Account)
                .Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == query.Id && x.UserId == userContext.Id, cancellationToken);
            return transaction is null
                ? TransactionErrors.NotFound(query.Id)
                : TransactionMapper.MapToDto(transaction);
        }
    }
}
