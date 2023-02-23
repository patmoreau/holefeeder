using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.FunctionalTests.Extensions;

internal static class EntityBuilderExtensions
{
    public static async Task<T> SavedInDb<T>(this IBuilder<T> builder, DbContextDriver databaseDriver)
        where T : class
    {
        T entity = builder.Build();
        await databaseDriver.SaveAsync(entity);
        return entity;
    }

    public static async Task<T[]> CollectionSavedInDb<T>(this ICollectionBuilder<T> builder,
        DbContextDriver databaseDriver, int count) where T : class
    {
        T[] entities = builder.Build(count);
        foreach (T entity in entities)
        {
            await databaseDriver.SaveAsync(entity);
        }

        return entities;
    }
}
