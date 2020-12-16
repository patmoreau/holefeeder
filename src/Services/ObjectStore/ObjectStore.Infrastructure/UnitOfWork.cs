using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoDbContext _context;

        public UnitOfWork(IMongoDbContext context)
        {
            _context = context.ThrowIfNull(nameof(context));
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Rollback()
        {
            _context.ClearCommands();
        }

        private bool _isDisposed;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _context.ClearCommands();
                _context.Dispose();
                
            }

            _isDisposed = true;
        }
    }
}
