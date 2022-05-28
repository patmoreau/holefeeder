namespace Holefeeder.Tests.Common.Builders;

internal interface IBuilder<out T> where T : class
{
    T Build();
}
