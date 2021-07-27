using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken);
    }
}
