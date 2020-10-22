using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Application.SeedWork;
using DrifterApps.Holefeeder.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Domain.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using DrifterApps.Holefeeder.Infrastructure.Database.Extensions;
using DrifterApps.Holefeeder.Infrastructure.Database.Schemas;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        
        public AccountRepository(IUnitOfWork unitOfWork, IMongoDbContext mongoDbContext, IMapper mapper)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(mapper));
            _mongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Account>> FindAsync(QueryParams queryParams, CancellationToken cancellationToken = default)
        {
            _ = queryParams ?? throw new ArgumentNullException(nameof(queryParams));
            
            var collection = await _mongoDbContext.GetAccountsAsync(cancellationToken);

            var accounts = await collection.FindAsync(queryParams, cancellationToken);
            
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
