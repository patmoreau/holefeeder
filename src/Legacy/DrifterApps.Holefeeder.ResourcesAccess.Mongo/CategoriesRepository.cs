using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class CategoriesRepository : BaseOwnedRepository<CategoryEntity, CategorySchema>, ICategoriesRepository
    {
        public CategoriesRepository(IMongoCollection<CategorySchema> collection, IMapper mapper) : base(collection, mapper)
        {
        }
    }
}