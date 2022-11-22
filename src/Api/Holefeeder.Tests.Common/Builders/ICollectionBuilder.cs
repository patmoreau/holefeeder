namespace Holefeeder.Tests.Common.Builders;

internal interface ICollectionBuilder<out T> where T : class
{
    T[] Build(int count);
}
