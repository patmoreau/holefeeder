namespace Holefeeder.Application.SeedWork;

public interface IRequestQuery
{
    int Offset { get; }
    int Limit { get; }
    string[] Sort { get; }
    string[] Filter { get; }
}
