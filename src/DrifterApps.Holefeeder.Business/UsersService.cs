using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Extensions;
using System.Threading;

namespace DrifterApps.Holefeeder.Business
{
    public class UsersService : BaseService<UserEntity>, IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository repository) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public Task<UserEntity> FindByEmailAsync(string emailAddress, CancellationToken cancellationToken = default) => _repository.FindByEmailAsync(emailAddress, cancellationToken);
    }
}
