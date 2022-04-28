namespace Holefeeder.Application.SeedWork;

public record QueryResult<TResult>(int Total, IEnumerable<TResult> Items);
