namespace Holefeeder.Domain.SeedWork;

public interface IUnitOfWork : IDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
