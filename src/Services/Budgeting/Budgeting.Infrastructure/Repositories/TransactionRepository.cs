using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Transaction = DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.TransactionContext.Transaction;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;


        public TransactionRepository(IUnitOfWork unitOfWork, IMongoDbContext mongoDbContext, IMapper mapper)
        {
            UnitOfWork = unitOfWork.ThrowIfNull(nameof(mapper));
            _mongoDbContext = mongoDbContext.ThrowIfNull(nameof(mongoDbContext));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<Transaction> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetTransactionsAsync(cancellationToken);
            var schema = collection.AsQueryable().Where(t => t.Id == id).SingleOrDefaultAsync(cancellationToken);

            return _mapper.Map<Transaction>(schema);
        }

        public async Task CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            var schema = _mapper.Map<TransactionSchema>(transaction);

            var collection = await _mongoDbContext.GetTransactionsAsync(cancellationToken);

            _mongoDbContext.AddCommand(async () =>
            {
                await collection.InsertOneAsync(schema, new InsertOneOptions {BypassDocumentValidation = false},
                    cancellationToken);
            });
        }

        public Task UpdateAsync(Transaction account, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsAccountActive(Guid id, CancellationToken cancellationToken)
        {
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            return await collection.AsQueryable()
                .AnyAsync(a => a.Id == id && !a.Inactive, cancellationToken: cancellationToken);
        }
    }
}
