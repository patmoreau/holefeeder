using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public static class MakePurchase
{
    public record Request
        : IRequest<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>, IValidateable
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        public Validator()
        {
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }

        public OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult> CreateResponse(
            ValidationResult result)
        {
            return new ValidationErrorsRequestResult(result.ToDictionary());
        }
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(ITransactionRepository transactionRepository, ItemsCache cache,
            ILogger<Handler> logger)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            try
            {
                var transaction = Transaction.Create(request.Date, request.Amount, request.Description,
                    request.CategoryId,
                    request.AccountId, (Guid)_cache["UserId"]);

                transaction = transaction.AddTags(request.Tags);

                _logger.LogInformation("----- Making Purchase - Transaction: {@Transaction}", transaction);

                await _transactionRepository.SaveAsync(transaction, cancellationToken);

                await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

                return transaction.Id;
            }
            catch (HolefeederDomainException ex)
            {
                _transactionRepository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}
