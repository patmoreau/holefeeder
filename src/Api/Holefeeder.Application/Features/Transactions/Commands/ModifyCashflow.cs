using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class ModifyCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/modify",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(ModifyCashflow))
            .RequireAuthorization();
    }

    public record Request : IRequest<Unit>
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public string[] Tags { get; init; } = Array.Empty<string>();
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Amount).GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly ILogger _logger;
        private readonly IUserContext _userContext;
        private readonly ICashflowRepository _cashflowRepository;

        public Handler(IUserContext userContext, ICashflowRepository cashflowRepository, ILogger<Handler> logger)
        {
            _userContext = userContext;
            _cashflowRepository = cashflowRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var exists =
                    await _cashflowRepository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
                if (exists is null)
                {
                    throw new CashflowNotFoundException(request.Id);
                }

                _logger.LogTrace("Existing {@Cashflow}", exists);

                var cashflow = exists with
                {
                    Amount = request.Amount,
                    Description = request.Description,
                };

                cashflow = cashflow.SetTags(request.Tags);

                _logger.LogTrace("Modifying {@Cashflow}", cashflow);

                await _cashflowRepository.SaveAsync(cashflow, cancellationToken);

                await _cashflowRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (TransactionDomainException)
            {
                _cashflowRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
