using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Categories;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class CategoriesQueriesRepository : ICategoryQueriesRepository, ICategoriesRepository
{
    private readonly IHolefeederContext _context;
    private readonly CategoryMapper _categoryMapper;

    public CategoriesQueriesRepository(IHolefeederContext context, CategoryMapper categoryMapper)
    {
        _context = context;
        _categoryMapper = categoryMapper;
    }

    public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string select = "SELECT * FROM categories WHERE user_id = @UserId ORDER BY name;";

        var connection = _context.Connection;

        var results = await connection
            .QueryAsync<CategoryEntity>(select, new { UserId = userId })
            .ConfigureAwait(false);

        return _categoryMapper.MapToDto(results);
    }

    public async Task<CategoryViewModel?> FindByNameAsync(Guid userId, string name,
        CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var category = (await connection.FindAsync<CategoryEntity>(new { UserId = userId, Name = name })
                .ConfigureAwait(false))
            .SingleOrDefault();

        return _categoryMapper.MapToDtoOrNull(category);
    }
}
