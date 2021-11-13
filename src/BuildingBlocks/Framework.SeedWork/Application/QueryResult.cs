using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application
{
    public interface IQueryResult
    {
        QueryStatus Status { get; }
        ImmutableArray<string>? Messages { get; }
    }

    public record QueryResult(QueryStatus Status, ImmutableArray<string>? Messages) : IQueryResult
    {
        public static QueryResult Ok() => new(QueryStatus.Ok, null);

        public static QueryResult NotFound() => new(QueryStatus.NotFound, null);

        public static QueryResult Errors(params string[] messages) =>
            new(QueryStatus.Error, ImmutableArray.Create(messages));
    }

    public record QueryResult<TResult>
        (QueryStatus Status, TResult? Item, ImmutableArray<string>? Messages) : IQueryResult
    {
        public static QueryResult<T> Found<T>(T result) => new(QueryStatus.Ok, result, null);

        public static QueryResult<T> NotFound<T>() => new(QueryStatus.NotFound, default, null);

        public static QueryResult<T> Errors<T>(params string[] messages) =>
            new(QueryStatus.Error, default, ImmutableArray.Create(messages));
    }

    public record QueryResultList<TResult>
        (QueryStatus Status, int Total, ImmutableArray<TResult> Items, ImmutableArray<string>? Messages) : IQueryResult
    {
        public static QueryResultList<T> Ok<T>(int total, IEnumerable<T> result) =>
            new(QueryStatus.Ok, total, result.ToImmutableArray(), null);

        public static QueryResultList<T> Errors<T>(params string[] messages) =>
            new(QueryStatus.Error, 0, ImmutableArray<T>.Empty, ImmutableArray.Create(messages));
    }
}
