namespace Holefeeder.Tests.Common.Builders;

internal interface IEntityBuilder<out T> where T : class
{
    T Build();
}
