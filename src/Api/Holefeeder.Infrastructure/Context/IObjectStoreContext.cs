using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.SeedWork;

namespace Holefeeder.Infrastructure.Context;

public interface IObjectStoreContext : IDbContext, IUnitOfWork
{
}
