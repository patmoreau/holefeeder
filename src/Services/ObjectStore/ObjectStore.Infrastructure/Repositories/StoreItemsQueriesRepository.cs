using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories
{
    public class StoreItemsQueriesRepository : IStoreItemsQueriesRepository
    {
        private readonly IObjectStoreContext _context;
        private readonly IMapper _mapper;

        public StoreItemsQueriesRepository(IObjectStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<QueryResult<StoreItemViewModel>> FindAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken)
        {
            if (queryParams is null)
            {
                throw new ArgumentNullException(nameof(queryParams));
            }

            return FindInternalAsync(userId, queryParams);
        }

        private async Task<QueryResult<StoreItemViewModel>> FindInternalAsync(Guid userId, QueryParams query)
        {
            var builder = new SqlBuilder();
            var selectTemplate =
                builder.AddTemplate(QUERY_TEMPLATE,
                    new { Offset = query.Offset + 1, Limit = query.Offset + query.Limit });
            var countTemplate = builder.AddTemplate(QUERY_COUNT_TEMPLATE);

            builder.Where($"user_id = @{nameof(userId)}", new { userId })
                .Filter(query.Filter)
                .Sort(query.Sort);

            var connection = _context.Connection;

            var items = await connection.QueryAsync<StoreItemEntity>(selectTemplate.RawSql, selectTemplate.Parameters);
            var count = await connection.ExecuteScalarAsync<int>(countTemplate.RawSql, countTemplate.Parameters);

            return new QueryResult<StoreItemViewModel>(count, _mapper.Map<IEnumerable<StoreItemViewModel>>(items));
        }

        public async Task<StoreItemViewModel> FindByIdAsync(Guid userId, Guid id,
            CancellationToken cancellationToken)
        {
            var connection = _context.Connection;

            var item = await connection
                .FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
                .ConfigureAwait(false);

            return _mapper.Map<StoreItemViewModel>(item);
        }

        public Task<bool> AnyCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
        {
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return AnyCodeInternalAsync(userId, code);
        }

        private async Task<bool> AnyCodeInternalAsync(Guid userId, string code)
        {
            var connection = _context.Connection;

            return await connection
                .QueryFirstOrDefaultAsync<StoreItemEntity>(SELECT_CODE, new { Code = code, UserId = userId })
                .ConfigureAwait(false) is not null;
        }

        public async Task<bool> AnyIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var connection = _context.Connection;

            return await connection
                .FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
                .ConfigureAwait(false) is not null;
        }

        private const string SELECT_CODE =
            "SELECT id, code, data, user_id FROM store_items WHERE lower(code) = lower(@Code) AND user_id = @UserId;";

        private const string QUERY_TEMPLATE = @"
SELECT X.* FROM (
    SELECT si.*, ROW_NUMBER() OVER (/**orderby**/) AS RowNumber 
    FROM store_items si 
    /**where**/
) AS X 
WHERE RowNumber BETWEEN @offset AND @limit";

        private const string QUERY_COUNT_TEMPLATE = @"SELECT COUNT(*) FROM store_items /**where**/";
    }
}
