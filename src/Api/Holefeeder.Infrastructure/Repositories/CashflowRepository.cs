using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class CashflowRepository : ICashflowRepository
{
    private readonly IHolefeederContext _context;

    public CashflowRepository(IHolefeederContext context)
    {
        _context = context;
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

        var cashflow = CashflowMapper.MapToModelOrNull(entity);

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
            await transaction.InsertAsync(CashflowMapper.MapToEntity(cashflow))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(CashflowMapper.MapToEntity(cashflow)).ConfigureAwait(false);
        }
    }
}
