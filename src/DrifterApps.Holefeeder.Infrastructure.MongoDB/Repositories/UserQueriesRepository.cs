using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories
{
    public class UserQueriesRepository : IUserQueriesRepository
    {
        private readonly IMongoDbContext _dbContext;

        public UserQueriesRepository(IMongoDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserViewModel> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var users = await _dbContext.GetUsersAsync(cancellationToken);

            var user = await users.AsQueryable().Where(u => u.EmailAddress == email)
                .SingleOrDefaultAsync(cancellationToken);

            if (user != null && user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
                await users.ReplaceOneAsync(u => u.MongoId == user.MongoId, user, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }

            return user == null
                ? null
                : new UserViewModel(user.Id, user.FirstName, user.LastName, user.EmailAddress, user.GoogleId,
                    user.DateJoined);
        }
    }
}
