using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.SeedWork;

namespace Holefeeder.FunctionalTests.Extensions;

internal static class EntityBuilderExtensions
{
    public static async Task<T> SavedInDb<T>(this RootBuilder<T> builder, DbContextDriver databaseDriver)
        where T : class
    {
        T entity = builder.Build();
        await databaseDriver.SaveAsync(entity);
        return entity;
    }

    public static async Task<IReadOnlyCollection<T>> CollectionSavedInDb<T>(this RootBuilder<T> builder,
        DbContextDriver databaseDriver, int? count = null) where T : class
    {
        var entities = builder.BuildCollection(count);
        foreach (T entity in entities)
        {
            await databaseDriver.SaveAsync(entity);
        }

        return entities;
    }
}
