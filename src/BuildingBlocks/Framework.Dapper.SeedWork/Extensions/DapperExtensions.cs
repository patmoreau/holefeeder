using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

using Humanizer;

namespace Framework.Dapper.SeedWork.Extensions
{
    public static class DapperExtensions
    {
        #region Insert

        public static Task<int> InsertAsync<T>(this IDbConnection connection, T entity) where T : class =>
            InsertAsync(connection, null, entity);

        public static Task<int> InsertAsync<T>(this IDbTransaction transaction, T entity) where T : class =>
            InsertAsync(transaction.Connection, transaction, entity);

        private static Task<int> InsertAsync<T>(IDbConnection connection, IDbTransaction transaction, T entity)
            where T : class
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var entityType = entity.GetType();
            var properties = GetAllProperties(entityType).ToList();
            var tableName = GetTableName(entityType);

            var sql = new StringBuilder($"INSERT INTO `{tableName}` (")
                .AppendJoin(", ", properties.Select(p => $"`{GetColumnName(p)}`"))
                .Append(") VALUES (")
                .AppendJoin(", ", properties.Select(p => $"@{p.Name}"))
                .Append(");").ToString();

            return connection.ExecuteAsync(sql, entity, transaction);
        }

        #endregion

        #region Update

        public static Task<int> UpdateAsync<T>(this IDbConnection connection, T entity) where T : class =>
            UpdateAsync(connection, null, entity);

        public static Task<int> UpdateAsync<T>(this IDbTransaction transaction, T entity) where T : class =>
            UpdateAsync(transaction.Connection, transaction, entity);

        private static Task<int> UpdateAsync<T>(IDbConnection connection, IDbTransaction transaction, T entity)
            where T : class
        {
            _ = entity ?? throw new ArgumentNullException(nameof(entity));

            var entityType = entity.GetType();

            var properties = GetProperties(entityType);
            var keys = GetIdProperties(entityType);
            var tableName = GetTableName(entityType);

            var sql = new StringBuilder($"UPDATE `{tableName}` SET ")
                .AppendJoin(", ", properties.Select(p => $"`{GetColumnName(p)}` = @{p.Name}"))
                .Append(" WHERE ")
                .AppendJoin(" AND ", keys.Select(k => $"`{GetColumnName(k)}` = @{k.Name}"))
                .Append(';').ToString();

            return connection.ExecuteAsync(sql, entity, transaction);
        }

        #endregion

        #region FindById

        public static Task<T> FindByIdAsync<T>(this IDbConnection connection, object param) where T : class =>
            FindByIdAsync<T>(connection, null, param);

        public static Task<T> FindByIdAsync<T>(this IDbTransaction transaction, object param) where T : class =>
            FindByIdAsync<T>(transaction.Connection, transaction, param);

        private static Task<T> FindByIdAsync<T>(IDbConnection connection, IDbTransaction transaction, object param)
            where T : class
        {
            _ = param ?? throw new ArgumentNullException(nameof(param));

            var entityType = typeof(T);
            var tableName = GetTableName(entityType);
            var keys = GetIdProperties(entityType).ToList();

            if (!keys.Any())
            {
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");
            }

            var sql = new StringBuilder($"SELECT * FROM `{tableName}` WHERE ")
                .AppendJoin(" AND ", keys.Select(p => $"`{GetColumnName(p)}` = @{p.Name}"))
                .Append(';')
                .ToString();

            var dynamicParameters = new DynamicParameters();
            if (keys.Count == 1)
                dynamicParameters.Add($"@{keys.First().Name}", param);
            else
            {
                foreach (var key in keys)
                    dynamicParameters.Add($"@{key.Name}", param.GetType().GetProperty(key.Name)?.GetValue(param, null));
            }

            return connection.QuerySingleOrDefaultAsync<T>(sql, dynamicParameters, transaction);
        }

        #endregion

        #region FindAsync

        public static Task<IEnumerable<T>> FindAsync<T>(this IDbConnection connection, object param = null)
            where T : class =>
            FindAsync<T>(connection, null, param);

        public static Task<IEnumerable<T>> FindAsync<T>(this IDbTransaction transaction, object param = null)
            where T : class =>
            FindAsync<T>(transaction.Connection, transaction, param);

        private static Task<IEnumerable<T>> FindAsync<T>(IDbConnection connection, IDbTransaction transaction,
            object param = null) where T : class
        {
            var entityType = typeof(T);
            var paramType = param?.GetType();

            var tableName = GetTableName(entityType);

            var sb = new StringBuilder($"SELECT * FROM `{tableName}`");
            if (paramType is not null)
            {
                var properties = GetAllProperties(paramType).ToList();

                sb.Append(" WHERE ")
                    .AppendJoin(" AND ", properties.Select(p => $"`{GetColumnName(p)}` = @{p.Name}"));
            }

            var sql = sb.Append(';').ToString();

            return connection.QueryAsync<T>(sql, param, transaction: transaction);
        }

        #endregion

        private static IEnumerable<PropertyDescriptor> GetAllProperties(Type type) =>
            TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>()
                .Where(p => !p.Attributes.OfType<NotMappedAttribute>().Any());

        private static IEnumerable<PropertyDescriptor> GetProperties(Type type) =>
            GetAllProperties(type).Where(p => !p.Attributes.OfType<KeyAttribute>().Any());

        private static IEnumerable<PropertyDescriptor> GetIdProperties(Type type)
        {
            var keys = GetAllProperties(type)
                .Where(p => p.Attributes.OfType<KeyAttribute>().Any())
                .ToList();

            return keys.Any()
                ? keys
                : TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>()
                    .Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetTableName(Type type) =>
            TypeDescriptor.GetAttributes(type).OfType<TableAttribute>().FirstOrDefault()?.Name ??
            type.Name.Underscore();

        private static string GetColumnName(MemberDescriptor property) =>
            property.Attributes.OfType<ColumnAttribute>().SingleOrDefault()?.Name ?? property.Name.Underscore();
    }
}
