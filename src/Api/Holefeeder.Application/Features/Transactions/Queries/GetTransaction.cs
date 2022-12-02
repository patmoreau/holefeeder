using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/transactions/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var requestResult = await mediator.Send(new Request(id), cancellationToken);
                    return Results.Ok(requestResult);
                })
            .Produces<TransactionInfoViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetTransaction))
            .RequireAuthorization();
    }

    internal record Request(Guid Id) : IRequest<TransactionInfoViewModel>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, TransactionInfoViewModel>
    {
        private readonly ITransactionQueriesRepository _repository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, ITransactionQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<TransactionInfoViewModel> Handle(Request query,
            CancellationToken cancellationToken)
        {
            var transaction = await _repository.FindByIdAsync(_userContext.UserId, query.Id, cancellationToken);
            if (transaction is null)
            {
                throw new TransactionNotFoundException(query.Id);
            }

            return transaction;
        }
    }
}
