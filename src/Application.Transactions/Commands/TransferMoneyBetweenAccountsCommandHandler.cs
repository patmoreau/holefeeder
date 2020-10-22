using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.SeedWork.Models;
using DrifterApps.Holefeeder.Domain.BoundedContext.TransactionContext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Application.Transactions.Commands
{
    public class TransferMoneyBetweenAccountsCommandHandler : IRequestHandler<TransferMoneyBetweenAccountsCommand, CommandResult<Guid>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger _logger;

        public TransferMoneyBetweenAccountsCommandHandler(ITransactionRepository transactionRepository,
            ILogger<MakePurchaseCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<CommandResult<Guid>> Handle(TransferMoneyBetweenAccountsCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            
            var errors = new List<string>();
            
            if (!(await _transactionRepository.IsAccountActive(request.DebitAccountId, cancellationToken)))
            {
                errors.Add($"Debit account {request.DebitAccountId} does not exists");
            }
            
            if (!(await _transactionRepository.IsAccountActive(request.CreditAccountId, cancellationToken)))
            {
                errors.Add($"Credit account {request.CreditAccountId} does not exists");
            }
            
            var transaction = Transaction.Create(request.Date, request.Amount, request.Description, Guid.Empty, request.DebitAccountId, null, null, request.UserId);

            _logger.LogInformation("----- Transfer Money Between Accounts - Transaction: {@Transaction}", transaction);

            await _transactionRepository.CreateAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return errors.Any() ? new CommandResult<Guid>(CommandStatus.BadRequest, Guid.Empty) : new CommandResult<Guid>(CommandStatus.Created, transaction.Id);
        }
    }
}
