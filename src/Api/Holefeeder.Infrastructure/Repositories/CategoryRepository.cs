using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class CategoryRepository : ICategoryRepository
{
    private readonly IHolefeederContext _context;

    public CategoryRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Category?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var category = await connection.FindByIdAsync<CategoryEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        return CategoryMapper.MapToModelOrNull(category);
    }

    public async Task<Category?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CategoryEntity>(new {Name = name, UserId = userId})
            .ConfigureAwait(false);

        return CategoryMapper.MapToModelOrNull(schema.FirstOrDefault());
    }

    public async Task SaveAsync(Category category, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = category.Id;
        var userId = category.UserId;

        var entity = await transaction.FindByIdAsync<CategoryEntity>(new {Id = id, UserId = userId});

        if (entity is null)
        {
            await transaction.InsertAsync(CategoryMapper.MapToEntity(category))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(CategoryMapper.MapToEntity(category)).ConfigureAwait(false);
        }
    }
}
