using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.Business
{
    public abstract class BaseOwnedService<TEntity> : BaseService<TEntity> where TEntity : BaseEntity, IIdentityEntity<TEntity>, IOwnedEntity<TEntity>
    {
        private readonly IBaseOwnedRepository<TEntity> _repository;

        protected BaseOwnedService(IBaseOwnedRepository<TEntity> repository) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public Task<bool> IsOwnerAsync(string userId, string id, CancellationToken cancellationToken = default) => _repository.IsOwnerAsync(userId, id, cancellationToken);

        public Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams, CancellationToken cancellationToken = default) => _repository.FindAsync(userId, queryParams, cancellationToken);

        public Task<TEntity> CreateAsync(string userId, TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.ThrowIfNull(nameof(entity));

            var newEntity = entity.WithUser(userId);

            return _repository.CreateAsync(newEntity, cancellationToken);
        }
    }
}
