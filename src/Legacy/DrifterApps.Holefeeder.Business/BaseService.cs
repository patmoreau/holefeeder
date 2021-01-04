using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using System.Threading;

namespace DrifterApps.Holefeeder.Business
{
    public abstract class BaseService<TEntity> where TEntity : BaseEntity, IIdentityEntity<TEntity>
    {
        private readonly IBaseRepository<TEntity> _repository;

        protected BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default) => _repository.RemoveAsync(id, cancellationToken);

        public Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.FindByIdAsync(id, cancellationToken);

        public Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams, CancellationToken cancellationToken = default) => _repository.FindAsync(queryParams, cancellationToken);

        public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default) => _repository.CreateAsync(entity, cancellationToken);

        public async Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
        {
            _ = await _repository.FindByIdAsync(id, cancellationToken).ConfigureAwait(false);

            entity.WithId(id);

            await _repository.UpdateAsync(id, entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
