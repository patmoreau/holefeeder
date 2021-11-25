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

public static class ModifyTransaction
{
    public record Request
        : IRequest<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>,
            IValidateable
    {
        public Guid Id { get; init; }

        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid AccountId { get; init; }

        public Guid CategoryId { get; init; }

        public string[] Tags { get; init; } = null!;
    }

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }

        public OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>
            CreateResponse(
                ValidationResult result)
            => new ValidationErrorsRequestResult(result.ToDictionary());
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public Handler(ITransactionRepository transactionRepository, ItemsCache cache,
            ILogger<Handler> logger)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, NotFoundRequestResult, Unit, DomainErrorRequestResult>>
            Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var exists =
                    await _transactionRepository.FindByIdAsync(request.Id, (Guid)_cache["UserId"], cancellationToken);
                if (exists is null)
                {
                    return new NotFoundRequestResult();
                }

                var transaction = exists with
                {
                    Date = request.Date,
                    Amount = request.Amount,
                    Description = request.Description,
                    CategoryId = request.CategoryId,
                    AccountId = request.AccountId
                };

                transaction = transaction.AddTags(request.Tags);

                _logger.LogInformation("----- Modifying - Transaction: {@Transaction}", transaction);

                await _transactionRepository.SaveAsync(transaction, cancellationToken);

                await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (HolefeederDomainException ex)
            {
                _transactionRepository.UnitOfWork.Dispose();
                return new DomainErrorRequestResult(ex.Context, ex.Message);
            }
        }
    }
}
