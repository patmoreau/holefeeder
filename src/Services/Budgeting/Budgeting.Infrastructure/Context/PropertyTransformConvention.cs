using Dapper.FluentMap.Conventions;

using Humanizer;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;

internal class PropertyTransformConvention : Convention
{
    public PropertyTransformConvention()
    {
        Properties().Configure(c => c.Transform(propertyName => propertyName.Underscore()));
    }
}
