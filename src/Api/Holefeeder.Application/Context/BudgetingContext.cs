using System.Data;

using Holefeeder.Domain.Features.Categories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Holefeeder.Application.Context;

public class BudgetingContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;

    public BudgetingContext(DbContextOptions<BudgetingContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder?.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type")
                .HasConversion(p => p.Name, p => CategoryType.FromName(p, false));
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.Favorite).HasColumnName("favorite");
            entity.Property(e => e.System).HasColumnName("system");
            entity.Property(e => e.BudgetAmount).HasColumnName("budget_amount");
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
