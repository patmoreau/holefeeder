using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class TransactionRepository : ITransactionRepository
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

    public async Task<bool> AccountExists(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return (await _context.Connection.FindByIdAsync<AccountEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false)) is not null;
    }

    public async Task<bool> CategoryExists(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        return (await _context.Connection.FindByIdAsync<CategoryEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false)) is not null;
    }
}
