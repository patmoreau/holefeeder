namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public interface IQuery
{
    int Offset { get; }
    int Limit { get; }
    string[] Sort { get; }
    string[] Filter { get; }
}
