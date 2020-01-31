using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public abstract class BaseOwnedService<TEntity> : BaseService<TEntity> where TEntity : BaseEntity, IIdentityEntity<TEntity>, IOwnedEntity<TEntity>
    {
        private readonly IBaseOwnedRepository<TEntity> _repository;

        protected BaseOwnedService(IBaseOwnedRepository<TEntity> repository) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public async Task<bool> IsOwnerAsync(string userId, string id) => await _repository.IsOwnerAsync(userId, id);

        public async Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams) => await _repository.FindAsync(userId, queryParams);

        public async Task<TEntity> CreateAsync(string userId, TEntity entity)
        {
            var newEntity = entity.WithUser(userId);

            return await _repository.CreateAsync(newEntity);
        }
    }
}
