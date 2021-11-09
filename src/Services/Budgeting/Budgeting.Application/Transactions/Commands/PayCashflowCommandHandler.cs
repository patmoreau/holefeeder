using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands
{
    public class PayCashflowCommandHandler : IRequestHandler<PayCashflowCommand, CommandResult<Guid>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICashflowRepository _cashflowRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public PayCashflowCommandHandler(ITransactionRepository transactionRepository,
            ICashflowRepository cashflowRepository, ItemsCache cache, ILogger<PayCashflowCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _cashflowRepository = cashflowRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CommandResult<Guid>> Handle(PayCashflowCommand request, CancellationToken cancellationToken)
        {
            var cashflow =
                await _cashflowRepository.FindByIdAsync(request.CashflowId, (Guid)_cache["UserId"], cancellationToken);
            if (cashflow is null)
            {
                throw new ValidationException($"Cashflow '{request.CashflowId}' does not exists");
            }

            var transaction = Transaction.Create(request.Date, request.Amount, cashflow.Description,
                cashflow.CategoryId, cashflow.AccountId, request.CashflowId, request.CashflowDate, (Guid)_cache["UserId"]);

            _logger.LogInformation("----- Pay Cashflow - Transaction: {@Transaction}", transaction);

            await _transactionRepository.SaveAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return CommandResult<Guid>.Create(CommandStatus.Created, transaction.Id);
        }
    }
}
