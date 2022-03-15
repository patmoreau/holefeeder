using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public interface IQueryResult
{
    QueryStatus Status { get; }
    ImmutableArray<string>? Messages { get; }
}

public record QueryResult(QueryStatus Status, ImmutableArray<string>? Messages) : IQueryResult
{
    public static QueryResult Ok()
    {
        return new(QueryStatus.Ok, null);
    }

    public static QueryResult NotFound()
    {
        return new(QueryStatus.NotFound, null);
    }

    public static QueryResult Errors(params string[] messages)
    {
        return new(QueryStatus.Error, ImmutableArray.Create(messages));
    }
}

public record QueryResult<TResult>
    (QueryStatus Status, TResult? Item, ImmutableArray<string>? Messages) : IQueryResult
{
    public static QueryResult<T> Found<T>(T result)
    {
        return new(QueryStatus.Ok, result, null);
    }

    public static QueryResult<T> NotFound<T>()
    {
        return new(QueryStatus.NotFound, default, null);
    }

    public static QueryResult<T> Errors<T>(params string[] messages)
    {
        return new(QueryStatus.Error, default, ImmutableArray.Create(messages));
    }
}

public record QueryResultList<TResult>
    (QueryStatus Status, int Total, ImmutableArray<TResult> Items, ImmutableArray<string>? Messages) : IQueryResult
{
    public static QueryResultList<T> Ok<T>(int total, IEnumerable<T> result)
    {
        return new(QueryStatus.Ok, total, result.ToImmutableArray(), null);
    }

    public static QueryResultList<T> Errors<T>(params string[] messages)
    {
        return new(QueryStatus.Error, 0, ImmutableArray<T>.Empty, ImmutableArray.Create(messages));
    }
}
