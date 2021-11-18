using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Categories;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class CategoriesQueriesRepository : ICategoryQueriesRepository, ICategoriesRepository
{
    private readonly IHolefeederContext _context;
    private readonly IMapper _mapper;

    public CategoriesQueriesRepository(IHolefeederContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string select = "SELECT * FROM categories WHERE user_id = @UserId ORDER BY name;";

        var connection = _context.Connection;

        var results = await connection
            .QueryAsync<CategoryEntity>(select, new { UserId = userId })
            .ConfigureAwait(false);

        return _mapper.Map<IEnumerable<CategoryViewModel>>(results);
    }

    public async Task<CategoryViewModel?> FindByNameAsync(Guid userId, string name,
        CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var category = (await connection.FindAsync<CategoryEntity>(new { UserId = userId, Name = name })
                .ConfigureAwait(false))
            .SingleOrDefault();

        return _mapper.Map<CategoryViewModel>(category);
    }
}
