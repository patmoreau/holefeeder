using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IHolefeederContext _context;
    private readonly IMapper _mapper;

    public IUnitOfWork UnitOfWork => _context;

    public TransactionRepository(IHolefeederContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Transaction?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken) =>
        _mapper.Map<Transaction>(await _context.Connection
            .FindByIdAsync<TransactionEntity>(new { Id = id, UserId = userId })
            .ConfigureAwait(false));

    public async Task SaveAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var id = transaction.Id;
        var userId = transaction.UserId;

        var connection = _context.Transaction;

        var entity = await connection.FindByIdAsync<TransactionEntity>(new { Id = id, UserId = userId })
            .ConfigureAwait(false);

        if (entity is null)
        {
            entity = _mapper.Map<TransactionEntity>(transaction);
            await _context.Transaction.InsertAsync(entity).ConfigureAwait(false);
        }
        else
        {
            entity = _mapper.Map(transaction, entity);
            await _context.Transaction.UpdateAsync(entity).ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Transaction;

        await connection.DeleteByIdAsync<TransactionEntity>(new { Id = id, UserId = userId })
            .ConfigureAwait(false);
    }
}
