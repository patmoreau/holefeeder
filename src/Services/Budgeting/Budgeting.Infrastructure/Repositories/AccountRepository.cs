using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Framework.Mongo.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        
        public AccountRepository(IUnitOfWork unitOfWork, IMongoDbContext mongoDbContext, IMapper mapper)
        {
            UnitOfWork = unitOfWork.ThrowIfNull(nameof(mapper));
            _mongoDbContext = mongoDbContext.ThrowIfNull(nameof(mongoDbContext));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<IEnumerable<Account>> FindAsync(QueryParams queryParams, CancellationToken cancellationToken = default)
        {
            queryParams.ThrowIfNull(nameof(queryParams));
            
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            var accounts = await collection
                .AsQueryable()
                .Filter(queryParams.Filter)
                .Sort(queryParams.Sort)
                .Offset(queryParams.Offset)
                .Limit(queryParams.Limit)
                .ToListAsync(cancellationToken);
            
            return _mapper.Map<IEnumerable<Account>>(accounts);
        }

        public Task<Account> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Account account, CancellationToken cancellationToken = default)
        {
            var schema = _mapper.Map<AccountSchema>(account);

            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            _mongoDbContext.AddCommand(async () =>
            {
                await collection.InsertOneAsync(schema, new InsertOneOptions {BypassDocumentValidation = false},
                    cancellationToken);
            });
        }

        public Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
