using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class CashflowRepository : ICashflowRepository
    {
        private readonly IHolefeederContext _context;
        private readonly IMapper _mapper;

        public IUnitOfWork UnitOfWork => _context;

        public CashflowRepository(IHolefeederContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Cashflow> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var connection = _context.Connection;

            var category = await connection.FindByIdAsync<CashflowEntity>(new { Id = id, UserId = userId })
                .ConfigureAwait(false);

            return _mapper.Map<Cashflow>(category);
        }

        public async Task SaveAsync(Cashflow cashflow, CancellationToken cancellationToken)
        {
            var transaction = _context.Transaction;

            var id = cashflow.Id;
            var userId = cashflow.UserId;

            var entity = await FindAsync(transaction.Connection, id, userId);

            if (entity is null)
            {
                await transaction.InsertAsync(_mapper.Map<CashflowEntity>(cashflow))
                    .ConfigureAwait(false);
            }
            else
            {
                await transaction.UpdateAsync(_mapper.Map(cashflow, entity)).ConfigureAwait(false);
            }
        }

        private static async Task<CashflowEntity> FindAsync(IDbConnection connection, Guid id, Guid userId)
        {
            var schema = await connection
                .FindByIdAsync<CashflowEntity>(new {Id = id, UserId = userId})
                .ConfigureAwait(false);
            return schema;
        }
    }
}
