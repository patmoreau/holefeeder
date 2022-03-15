using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.CategoryContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CategoryMapper _categoryMapper;
    private readonly IHolefeederContext _context;

    public CategoryRepository(IHolefeederContext context, CategoryMapper categoryMapper)
    {
        _context = context;
        _categoryMapper = categoryMapper;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Category?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var category = await connection.FindByIdAsync<CategoryEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        return _categoryMapper.MapToModelOrNull(category);
    }

    public async Task<Category?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CategoryEntity>(new {Name = name, UserId = userId})
            .ConfigureAwait(false);

        return _categoryMapper.MapToModelOrNull(schema.FirstOrDefault());
    }

    public async Task SaveAsync(Category category, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = category.Id;
        var userId = category.UserId;

        var entity = await transaction.FindByIdAsync<CategoryEntity>(new {Id = id, UserId = userId});

        if (entity is null)
        {
            await transaction.InsertAsync(_categoryMapper.MapToEntity(category))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(_categoryMapper.MapToEntity(category)).ConfigureAwait(false);
        }
    }
}
