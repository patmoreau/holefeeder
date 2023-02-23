using System.Data;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Holefeeder.Application.Context;

public sealed class BudgetingContext : DbContext, IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;

    public BudgetingContext(DbContextOptions<BudgetingContext> options) : base(options) =>
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

    public DbSet<Account> Accounts { get; set; } = default!;
    public DbSet<Cashflow> Cashflows { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<StoreItem> StoreItems { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction =
            await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        new CategoryEntityTypeConfiguration().Configure(modelBuilder.Entity<Category>());
        new AccountEntityTypeConfiguration().Configure(modelBuilder.Entity<Account>());
        new CashflowEntityTypeConfiguration().Configure(modelBuilder.Entity<Cashflow>());
        new StoreItemEntityTypeConfiguration().Configure(modelBuilder.Entity<StoreItem>());
        new TransactionEntityTypeConfiguration().Configure(modelBuilder.Entity<Transaction>());
    }
}
