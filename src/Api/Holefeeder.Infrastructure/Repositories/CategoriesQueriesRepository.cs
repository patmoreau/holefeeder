using Dapper;

using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.Models;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class CategoriesQueriesRepository : ICategoryQueriesRepository, ICategoriesRepository
{
    private readonly IHolefeederContext _context;

    public CategoriesQueriesRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public async Task<CategoryViewModel?> FindByNameAsync(Guid userId, string name,
        CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var category = (await connection.FindAsync<CategoryEntity>(new {UserId = userId, Name = name})
                .ConfigureAwait(false))
            .SingleOrDefault();

        return CategoryMapper.MapToDtoOrNull(category);
    }

    public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string select = "SELECT * FROM categories WHERE user_id = @UserId ORDER BY name;";

        var connection = _context.Connection;

        var results = await connection
            .QueryAsync<CategoryEntity>(select, new {UserId = userId})
            .ConfigureAwait(false);

        return CategoryMapper.MapToDto(results);
    }
}
