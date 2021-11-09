using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands
{
    public class MakePurchaseCommandHandler : IRequestHandler<MakePurchaseCommand, CommandResult<Guid>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ItemsCache _cache;
        private readonly ILogger _logger;

        public MakePurchaseCommandHandler(ITransactionRepository transactionRepository, ItemsCache cache,
            ILogger<MakePurchaseCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CommandResult<Guid>> Handle(MakePurchaseCommand request, CancellationToken cancellationToken)
        {
            var transaction = Transaction.Create(request.Date, request.Amount, request.Description, request.CategoryId,
                request.AccountId, (Guid)_cache["UserId"]);

            transaction = transaction.AddTags(request.Tags);
            
            _logger.LogInformation("----- Making Purchase - Transaction: {@Transaction}", transaction);

            await _transactionRepository.SaveAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return CommandResult<Guid>.Create(CommandStatus.Created, transaction.Id);
        }
    }
}
