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

public class CashflowRepository : ICashflowRepository
{
    private readonly CashflowMapper _cashflowMapper;
    private readonly IHolefeederContext _context;

    public CashflowRepository(IHolefeederContext context, CashflowMapper cashflowMapper)
    {
        _context = context;
        _cashflowMapper = cashflowMapper;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Cashflow?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var entity = await connection.FindByIdAsync<CashflowEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        if (entity is null)
        {
            return null;
        }

        var cashflow = _cashflowMapper.MapToModelOrNull(entity);

        return cashflow;
    }

    public async Task SaveAsync(Cashflow cashflow, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = cashflow.Id;
        var userId = cashflow.UserId;

        var entity = await transaction.FindByIdAsync<CashflowEntity>(new {Id = id, UserId = userId});

        if (entity is null)
        {
            await transaction.InsertAsync(_cashflowMapper.MapToEntity(cashflow))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(_cashflowMapper.MapToEntity(cashflow)).ConfigureAwait(false);
        }
    }
}
