using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class TransactionsRepository : BaseOwnedRepository<TransactionEntity, TransactionSchema>, ITransactionsRepository
    {
        public TransactionsRepository(IMongoCollection<TransactionSchema> transactions, IMapper mapper) : base(transactions, mapper)
        {
        }
    }
}