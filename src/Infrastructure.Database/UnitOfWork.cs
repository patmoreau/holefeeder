using System;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;

namespace DrifterApps.Holefeeder.Infrastructure.Database
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
            }

            _isDisposed = true;
        }
    }
}
