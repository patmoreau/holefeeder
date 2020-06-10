using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DrifterApps.Holefeeder.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken);
        void Rollback();
    }
}
