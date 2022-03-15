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
using Dapper.Transaction;

using Humanizer;

namespace Framework.Dapper.SeedWork.Extensions;

public static class DapperExtensions
{
    private static IEnumerable<PropertyDescriptor> GetAllProperties(Type type)
    {
        return TypeDescriptor.GetProperties(type).OfType<PropertyDescriptor>()
            .Where(p => !p.Attributes.OfType<NotMappedAttribute>().Any());
    }

    private static IEnumerable<PropertyDescriptor> GetProperties(Type type)
    {
        return GetAllProperties(type).Where(p => !p.Attributes.OfType<KeyAttribute>().Any());
    }

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

    private static string GetTableName(Type type)
    {
        return TypeDescriptor.GetAttributes(type).OfType<TableAttribute>().FirstOrDefault()?.Name ??
               type.Name.Underscore();
    }

    private static string GetColumnName(MemberDescriptor property)
    {
        return property.Attributes.OfType<ColumnAttribute>().SingleOrDefault()?.Name ?? property.Name.Underscore();
    }

    #region Insert

    public static Task<int> InsertAsync<T>(this IDbConnection connection, T entity) where T : class
    {
        var sql = BuildInsertSql(entity);
        return connection.ExecuteAsync(sql, entity);
    }

    public static Task<int> InsertAsync<T>(this IDbTransaction transaction, T entity) where T : class
    {
        var sql = BuildInsertSql(entity);
        return transaction.ExecuteAsync(sql, entity);
    }

    private static string BuildInsertSql<T>(T entity)
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

        return sql;
    }

    #endregion

    #region Update

    public static Task<int> UpdateAsync<T>(this IDbConnection connection, T entity) where T : class
    {
        var sql = BuildUpdateSql(entity);
        return connection.ExecuteAsync(sql, entity);
    }

    public static Task<int> UpdateAsync<T>(this IDbTransaction transaction, T entity) where T : class
    {
        var sql = BuildUpdateSql(entity);
        return transaction.ExecuteAsync(sql, entity);
    }

    private static string BuildUpdateSql<T>(T entity) where T : class
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

        return sql;
    }

    #endregion

    #region FindById

    public static Task<T?> FindByIdAsync<T>(this IDbConnection connection, object param) where T : class
    {
        var (sql, dynamicParameters) = BuildFindByIdSql<T>(param);
        return connection.QuerySingleOrDefaultAsync<T?>(sql, dynamicParameters);
    }

    public static Task<T?> FindByIdAsync<T>(this IDbTransaction transaction, object param) where T : class
    {
        var (sql, dynamicParameters) = BuildFindByIdSql<T>(param);
        return transaction.QuerySingleOrDefaultAsync<T?>(sql, dynamicParameters);
    }

    private static (string, DynamicParameters) BuildFindByIdSql<T>(object param)
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
        {
            dynamicParameters.Add($"@{keys.First().Name}", param);
        }
        else
        {
            foreach (var key in keys)
            {
                dynamicParameters.Add($"@{key.Name}", param.GetType().GetProperty(key.Name)?.GetValue(param, null));
            }
        }

        return (sql, dynamicParameters);
    }

    #endregion

    #region FindAsync

    public static Task<IEnumerable<T>> FindAsync<T>(this IDbConnection connection, object? param = null)
        where T : class
    {
        var sql = BuildFindSql<T>(param);
        return connection.QueryAsync<T>(sql, param);
    }

    public static Task<IEnumerable<T>> FindAsync<T>(this IDbTransaction transaction, object? param = null)
        where T : class
    {
        var sql = BuildFindSql<T>(param);
        return transaction.QueryAsync<T>(sql, param);
    }

    private static string BuildFindSql<T>(object? param = null) where T : class
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

        return sql;
    }

    #endregion

    #region DeleteById

    public static Task<int> DeleteByIdAsync<T>(this IDbConnection connection, object param) where T : class
    {
        var (sql, dynamicParameters) = BuildDeleteByIdSql<T>(param);
        return connection.ExecuteAsync(sql, dynamicParameters);
    }

    public static Task<int> DeleteByIdAsync<T>(this IDbTransaction transaction, object param) where T : class
    {
        var (sql, dynamicParameters) = BuildDeleteByIdSql<T>(param);
        return transaction.ExecuteAsync(sql, dynamicParameters);
    }

    private static (string, DynamicParameters) BuildDeleteByIdSql<T>(object param)
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

        var sql = new StringBuilder($"DELETE FROM `{tableName}` WHERE ")
            .AppendJoin(" AND ", keys.Select(p => $"`{GetColumnName(p)}` = @{p.Name}"))
            .Append(';')
            .ToString();

        var dynamicParameters = new DynamicParameters();
        if (keys.Count == 1)
        {
            dynamicParameters.Add($"@{keys.First().Name}", param);
        }
        else
        {
            foreach (var key in keys)
            {
                dynamicParameters.Add($"@{key.Name}", param.GetType().GetProperty(key.Name)?.GetValue(param, null));
            }
        }

        return (sql, dynamicParameters);
    }

    #endregion
}
