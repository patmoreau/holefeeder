using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.FunctionalTests.Extensions;

internal static class EntityBuilderExtensions
{
    public static async Task<T> SavedInDb<T>(this IEntityBuilder<T> builder, DatabaseDriver databaseDriver)
        where T : class
    {
        var entity = builder.Build();
        await databaseDriver.Save(entity);
        return entity;
    }
}
