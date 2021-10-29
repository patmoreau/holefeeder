using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public interface IHolefeederContext : IDbContext, IUnitOfWork
    {
    }
}
