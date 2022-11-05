using System.Data;

using Holefeeder.Domain.Features.StoreItem;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Holefeeder.Application.Context;

public class StoreItemContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;

    public StoreItemContext(DbContextOptions<StoreItemContext> options) : base(options)
    {
    }

    public DbSet<StoreItem> StoreItems { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder?.Entity<StoreItem>(entity =>
        {
            entity.ToTable("store_items");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction =
            await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken: cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
