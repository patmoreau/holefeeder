using System;
using System.Data;

using Dapper;

using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

using Framework.Dapper.SeedWork.Extensions;

using MySqlConnector;

namespace ObjectStore.FunctionalTests
{
    public static class StoreItemContextSeed
    {
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();
        public static readonly Guid TestUserGuid3 = Guid.NewGuid();

        public static readonly Guid Guid1 = Guid.NewGuid();
        public static readonly Guid Guid2 = Guid.NewGuid();
        public static readonly Guid Guid3 = Guid.NewGuid();
        public static readonly Guid Guid4 = Guid.NewGuid();

        public static void PrepareData(ObjectStoreDatabaseSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var settingsBuilder = new MySqlConnectionStringBuilder(settings.ConnectionString);
            var databaseName = settingsBuilder.Database;
            settingsBuilder.Database = String.Empty;

            using var connection = new MySqlConnection(settingsBuilder.ConnectionString);

            connection.Open();

            RefreshDb(connection, databaseName);

            connection.Close();
        }

        public static void SeedData(ObjectStoreDatabaseSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            using (var connection = new MySqlConnection(settings.ConnectionString))
            {
                connection.Open();

                CreateStoreItem(connection, Guid1, 1, TestUserGuid1);
                CreateStoreItem(connection, Guid2, 2, TestUserGuid1);
                CreateStoreItem(connection, Guid3, 3, TestUserGuid1);
                CreateStoreItem(connection, Guid4, 4, TestUserGuid2);
                for (var x = 1; x <= 100; x++)
                {
                    CreateStoreItem(connection, Guid.NewGuid(), x, TestUserGuid3);
                }

                connection.Close();
            }
        }

        private static void RefreshDb(IDbConnection connection, string databaseName)
        {
            connection.Execute($"DROP DATABASE IF EXISTS {databaseName};");
        }

        private static void CreateStoreItem(IDbConnection connection, Guid id, int index, Guid userId)
        {
            connection.InsertAsync(new StoreItemEntity
            {
                Id = id, Code = $"Code{index:000}", Data = $"Data{index:000}", UserId = userId
            }).Wait();
        }
    }
}
