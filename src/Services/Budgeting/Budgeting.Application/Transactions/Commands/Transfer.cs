using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public static class Transfer
{
    public record Request
        : IRequest<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>, IValidateable
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public string Description { get; init; } = null!;

        public Guid FromAccountId { get; init; }

        public Guid ToAccountId { get; init; }
    }

    public class Validator : AbstractValidator<Request>,
        IValidator<Request, OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        public Validator()
        {
            RuleFor(command => command.FromAccountId).NotNull().NotEmpty();
            RuleFor(command => command.ToAccountId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
            RuleFor(command => command.Amount).GreaterThan(0);
        }

        public OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult> CreateResponse(
            ValidationResult result)
        {
            return new ValidationErrorsRequestResult(result.ToDictionary());
        }
    }

    public class
        Handler : IRequestHandler<Request, OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        private readonly IAccountQueriesRepository _accountQueriesRepository;
        private readonly ItemsCache _cache;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ILogger _logger;
        private readonly ITransactionRepository _transactionRepository;

        public Handler(ITransactionRepository transactionRepository,
            IAccountQueriesRepository accountQueriesRepository,
            ICategoriesRepository categoriesRepository,
            ItemsCache cache,
            ILogger<Handler> logger)
        {
            _transactionRepository = transactionRepository;
            _accountQueriesRepository = accountQueriesRepository;
            _categoriesRepository = categoriesRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var errors = new List<string>();
            var userId = (Guid)_cache["UserId"];

            if (!await _accountQueriesRepository.IsAccountActive(request.FromAccountId, userId,
                    cancellationToken))
            {
                errors.Add($"From account {request.FromAccountId} does not exists");
            }

            if (!await _accountQueriesRepository.IsAccountActive(request.ToAccountId, userId,
                    cancellationToken))
            {
                errors.Add($"To account {request.ToAccountId} does not exists");
            }

            var transferTo = await _categoriesRepository.FindByNameAsync(userId, "Transfer In", cancellationToken);
            var transferFrom = await _categoriesRepository.FindByNameAsync(userId, "Transfer Out", cancellationToken);

            var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description,
                transferFrom!.Id,
                request.FromAccountId, userId);

            _logger.LogInformation("----- Transfer Money from Account - Transaction: {@Transaction}", transactionFrom);

            await _transactionRepository.SaveAsync(transactionFrom, cancellationToken);

            var transactionTo = Transaction.Create(request.Date, request.Amount, request.Description, transferTo!.Id,
                request.ToAccountId, userId);

            _logger.LogInformation("----- Transfer Money to Account - Transaction: {@Transaction}", transactionTo);

            await _transactionRepository.SaveAsync(transactionTo, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return errors.Any() ? Guid.Empty : transactionFrom.Id;
        }
    }
}
