using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Repositories
{
    public class UserQueriesRepository : IUserQueriesRepository
    {
        private readonly IMongoDbContext _dbContext;

        public UserQueriesRepository(IMongoDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<UserViewModel> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var userCollection = await _dbContext.GetUsersAsync(cancellationToken);

            var user = await userCollection.AsQueryable().Where(u => u.EmailAddress == email)
                .SingleOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return null;
            }

            // ReSharper disable once InvertIf
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
                await userCollection.ReplaceOneAsync(u => u.MongoId == user.MongoId, user, cancellationToken: cancellationToken)
                    ;
            }

            return new UserViewModel(user.Id, user.FirstName, user.LastName, user.EmailAddress, user.GoogleId,
                    user.DateJoined);
        }
    }
}
