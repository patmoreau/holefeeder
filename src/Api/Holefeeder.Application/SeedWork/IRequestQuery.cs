namespace Holefeeder.Application.SeedWork;

public interface IRequestQuery
{
    int Offset { get; }
    int Limit { get; }
#pragma warning disable CA1819
    string[] Sort { get; }
    string[] Filter { get; }
#pragma warning restore CA1819
}
