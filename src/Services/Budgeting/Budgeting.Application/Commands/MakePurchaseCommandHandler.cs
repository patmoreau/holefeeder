using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Framework.SeedWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Commands
{
    public class MakePurchaseCommandHandler : IRequestHandler<MakePurchaseCommand, bool>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger _logger;

        public MakePurchaseCommandHandler(ITransactionRepository transactionRepository,
            ILogger<MakePurchaseCommandHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(MakePurchaseCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            var transaction = Transaction.Create(request.Date, request.Amount, request.Description, request.CategoryId, request.AccountId, null, null, request.UserId);

            foreach (var item in request.Tags)
            {
                transaction.AddTag(item);
            }

            _logger.LogInformation("----- Making Purchase - Transaction: {@Transaction}", transaction);

            await _transactionRepository.CreateAsync(transaction, cancellationToken);

            await _transactionRepository.UnitOfWork.CommitAsync(cancellationToken);

            return true;
        }
    }
}
