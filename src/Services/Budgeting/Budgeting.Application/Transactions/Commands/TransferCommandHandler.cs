using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;

using MediatR;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Transactions.Commands;

public class
    TransferCommandHandler : IRequestHandler<TransferCommand, Guid>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountQueriesRepository _accountQueriesRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly ItemsCache _cache;
    private readonly ILogger _logger;

    public TransferCommandHandler(ITransactionRepository transactionRepository,
        IAccountQueriesRepository accountQueriesRepository,
        ICategoriesRepository categoriesRepository,
        ItemsCache cache,
        ILogger<MakePurchaseCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _accountQueriesRepository = accountQueriesRepository;
        _categoriesRepository = categoriesRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Guid> Handle(TransferCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var userId = (Guid)_cache["UserId"];

        if (!(await _accountQueriesRepository.IsAccountActive(request.FromAccountId, userId,
                cancellationToken)))
        {
            errors.Add($"From account {request.FromAccountId} does not exists");
        }

        if (!(await _accountQueriesRepository.IsAccountActive(request.ToAccountId, userId,
                cancellationToken)))
        {
            errors.Add($"To account {request.ToAccountId} does not exists");
        }

        var transferTo = await _categoriesRepository.FindByNameAsync(userId, "Transfer In", cancellationToken);
        var transferFrom = await _categoriesRepository.FindByNameAsync(userId, "Transfer Out", cancellationToken);

        var transactionFrom = Transaction.Create(request.Date, request.Amount, request.Description, transferFrom!.Id,
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
