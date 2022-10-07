using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class DeleteTransaction : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/v2/transactions/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new Request(id), cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(DeleteTransaction))
            .RequireAuthorization();
    }

    internal record Request(Guid Id) : IRequest<Unit>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, ITransactionRepository transactionRepository)
        {
            _userContext = userContext;
            _transactionRepository = transactionRepository;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var transaction =
                await _transactionRepository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
            if (transaction is null)
            {
                throw new TransactionNotFoundException(request.Id);
            }

            await _transactionRepository.DeleteAsync(request.Id, _userContext.UserId, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
