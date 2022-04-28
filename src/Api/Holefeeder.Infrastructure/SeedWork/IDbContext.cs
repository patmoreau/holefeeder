using System.Data;

namespace Holefeeder.Infrastructure.SeedWork;

public interface IDbContext : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
}
