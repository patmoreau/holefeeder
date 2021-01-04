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
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();//.Parse("5fb92a48-cfb1-4508-b943-cada9f833f0e");
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();//.Parse("14e5d3a2-ca8a-4061-99b2-40aa957cc5db");

        public static readonly Guid Guid1 = Guid.NewGuid();//.Parse("50c21a9e-3d66-4a87-b533-2dc1a1f0d2d8");
        public static readonly Guid Guid2 = Guid.NewGuid();//.Parse("633f12e1-a558-42da-8a48-612401adb41c");
        public static readonly Guid Guid3 = Guid.NewGuid();//.Parse("9bb1d83a-cf26-40f5-95f6-f2fb6a20a94e");
        public static readonly Guid Guid4 = Guid.NewGuid();//.Parse("e8df8e7f-9341-4e61-8457-2c9b012dd7f3");

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
