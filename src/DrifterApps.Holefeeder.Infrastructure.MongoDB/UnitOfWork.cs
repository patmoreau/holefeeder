using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Domain.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoDbContext _context;

        public UnitOfWork(IMongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Rollback()
        {
            _context.ClearCommands();
        }

        public void Dispose()
        {
            _context.ClearCommands();
            GC.SuppressFinalize(this);
        }
    }
}
