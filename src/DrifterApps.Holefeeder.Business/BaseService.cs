using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public abstract class BaseService<TEntity> where TEntity : BaseEntity, IIdentityEntity<TEntity>
    {
        private readonly IBaseRepository<TEntity> _repository;

        protected BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public async Task DeleteAsync(string id) => await _repository.RemoveAsync(id);

        public async Task<TEntity> FindByIdAsync(string id) => await _repository.FindByIdAsync(id);

        public async Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams) => await _repository.FindAsync(queryParams);

        public async Task<TEntity> CreateAsync(TEntity entity) => await _repository.CreateAsync(entity);

        public async Task UpdateAsync(string id, TEntity entity)
        {
            var oldEntity = await _repository.FindByIdAsync(id);

            entity.WithId(id);

            await _repository.UpdateAsync(id, entity);
        }
    }
}
