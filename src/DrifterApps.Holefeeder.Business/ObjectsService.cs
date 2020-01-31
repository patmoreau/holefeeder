using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public class ObjectsService : BaseOwnedService<ObjectDataEntity>, IObjectsService
    {
        private readonly IObjectsRepository _repository;
        private readonly IMapper _mapper;

        public ObjectsService(IObjectsRepository repository, IMapper mapper) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<ObjectDataEntity> FindByCodeAsync(string userId, string code) => _mapper.Map<ObjectDataEntity>(await _repository.FindByCodeAsync(userId, code));
    }
}