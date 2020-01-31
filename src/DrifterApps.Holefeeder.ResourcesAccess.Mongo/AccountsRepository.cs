using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class AccountsRepository : BaseOwnedRepository<AccountEntity, AccountSchema>, IAccountsRepository
    {
        public AccountsRepository(IMongoCollection<AccountSchema> accounts, IMapper mapper) : base(accounts, mapper)
        {
        }
    }
}