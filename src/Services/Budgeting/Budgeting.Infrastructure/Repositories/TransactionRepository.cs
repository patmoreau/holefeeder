using System;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IHolefeederContext _context;
    private readonly TransactionMapper _transactionMapper;

    public TransactionRepository(IHolefeederContext context, TransactionMapper transactionMapper)
    {
        _context = context;
        _transactionMapper = transactionMapper;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Transaction?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return _transactionMapper.MapToModelOrNull(await _context.Connection
            .FindByIdAsync<TransactionEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false));
    }

    public async Task SaveAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var id = transaction.Id;
        var userId = transaction.UserId;

        var connection = _context.Transaction;

        var entity = await connection.FindByIdAsync<TransactionEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        if (entity is null)
        {
            entity = _transactionMapper.MapToEntity(transaction);
            await _context.Transaction.InsertAsync(entity).ConfigureAwait(false);
        }
        else
        {
            entity = _transactionMapper.MapToEntity(transaction);
            await _context.Transaction.UpdateAsync(entity).ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Transaction;

        await connection.DeleteByIdAsync<TransactionEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);
    }
}
