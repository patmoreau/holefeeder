using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public class UsersService : BaseService<UserEntity>, IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository repository) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
        }

        public async Task<UserEntity> FindByEmailAsync(string emailAddress) => await _repository.FindByEmailAsync(emailAddress);
    }
}
