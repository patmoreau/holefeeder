using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

using OneOf;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public static class PayCashflow
{
    public record Request : IRequest<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        public DateTime Date { get; init; }

        public decimal Amount { get; init; }

        public Guid CashflowId { get; init; }

        public DateTime CashflowDate { get; init; }
    }

    public class Handler : IRequestHandler<Request,
        OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICashflowRepository _cashflowRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public Handler(ITransactionRepository transactionRepository,
            ICashflowRepository cashflowRepository, ItemsCache cache, ILogger<Handler> logger)
        {
            _transactionRepository = transactionRepository;
            _cashflowRepository = cashflowRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<OneOf<ValidationErrorsRequestResult, Guid, DomainErrorRequestResult>> Handle(Request request,
            CancellationToken cancellationToken)
        {
            var cashflow =
                await _cashflowRepository.FindByIdAsync(request.CashflowId, (Guid)_cache["UserId"], cancellationToken);
            if (cashflow is null)
            {
                throw new ValidationException($"Cashflow '{request.CashflowId}' does not exists");
            }

            var transaction = Transaction.Create(request.Date, request.Amount, cashflow.Description,
                cashflow.CategoryId, cashflow.AccountId, request.CashflowId, request.CashflowDate,
                (Guid)_cache["UserId"]);

            _logger.LogInformation("----- Pay Cashflow - Transaction: {@Transaction}", transaction);

            await _transactionRepository.SaveAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return transaction.Id;
        }
    }
}
