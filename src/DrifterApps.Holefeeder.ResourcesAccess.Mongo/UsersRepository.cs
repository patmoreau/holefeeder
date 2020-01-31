using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class UsersRepository : BaseRepository<UserEntity, UserSchema>, IUsersRepository
    {
        private readonly IMongoCollection<UserSchema> _collection;

        public UsersRepository(IMongoCollection<UserSchema> collection, IMapper mapper) : base(collection, mapper)
        {
            _collection = collection.ThrowIfNull(nameof(collection));
        }

        public async Task<UserEntity> FindByEmailAsync(string emailAddress) => Mapper.Map<UserEntity>(await _collection.AsQueryable().Where(x => x.EmailAddress == emailAddress).FirstOrDefaultAsync());
    }
}