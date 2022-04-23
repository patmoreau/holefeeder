using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.Infrastructure.Builders;

internal interface IEntityBuilder<out T> where T : class
{
    T Build();
}

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
