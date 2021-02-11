using System;
using System.Linq;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;
using MongoDB.Driver;

namespace ObjectStore.FunctionalTests
{
    public static class StoreItemContextSeed
    {
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();

        public static readonly Guid Guid1 = Guid.NewGuid();
        public static readonly Guid Guid2 = Guid.NewGuid();
        public static readonly Guid Guid3 = Guid.NewGuid();
        public static readonly Guid Guid4 = Guid.NewGuid();

        public static void SeedData(IHolefeederDatabaseSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));

            var client = new MongoClient(settings.ConnectionString);

            CleanupData(client, settings);

            var database = client.GetDatabase(settings.Database);

            var collection = database.GetCollection<StoreItemSchema>(StoreItemSchema.SCHEMA);

            CreateStoreItem(collection, Guid1, 1, TestUserGuid1);
            CreateStoreItem(collection, Guid2, 2, TestUserGuid1);
            CreateStoreItem(collection, Guid3, 3, TestUserGuid1);
            CreateStoreItem(collection, Guid4, 4, TestUserGuid2);
        }

        private static void CreateStoreItem(IMongoCollection<StoreItemSchema> collection, Guid id, int index,
            Guid userId)
        {
            collection.InsertOne(new StoreItemSchema()
            {
                Id = id, Code = $"Code{index}", Data = $"Data{index}", UserId = userId
            });
        }

        private static void CleanupData(IMongoClient client, IHolefeederDatabaseSettings settings)
        {
            var databases = client.ListDatabaseNames().ToList();
            if (databases.Any(db => settings.Database.Equals(db, StringComparison.OrdinalIgnoreCase)))
            {
                client.DropDatabase(settings.Database);
            }
        }
    }
}
