using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class CashflowsRepository : BaseOwnedRepository<CashflowEntity, CashflowSchema>, ICashflowsRepository
    {
        public CashflowsRepository(IMongoCollection<CashflowSchema> cashflows, IMapper mapper) : base(cashflows, mapper)
        {
        }
    }
}