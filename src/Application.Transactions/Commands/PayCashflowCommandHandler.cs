using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Application.Transactions.Commands
{
    public class PayCashflowCommandHandler : IRequestHandler<PayCashflowCommand, bool>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger _logger;

        public PayCashflowCommandHandler(ITransactionRepository transactionRepository,
            ILogger<PayCashflowCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(PayCashflowCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));
            
            var transaction = Transaction.Create(request.Date, request.Amount, request.Description, request.CategoryId,
                request.AccountId, request.CashflowId, request.CashflowDate, request.UserId);

            foreach (var item in request.Tags)
            {
                transaction.AddTag(item);
            }

            _logger.LogInformation("----- Pay Cashflow - Transaction: {@Transaction}", transaction);

            await _transactionRepository.CreateAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return true;
        }
    }
}
