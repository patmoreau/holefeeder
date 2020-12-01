using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.Budgeting.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken);
        void Rollback();
    }
}
