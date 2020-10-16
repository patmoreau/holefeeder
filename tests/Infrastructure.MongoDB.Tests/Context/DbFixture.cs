using System;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Tests.Context
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbFixture : IDisposable
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;

        public MongoDbContext DbContext { get; }

        public DbFixture()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<DbFixture>()
                .Build();

            var mongoConfig = config.GetSection("MongoDB");
            var connString = mongoConfig["ConnectionString"];
            _databaseName = $"test_db_{Guid.NewGuid()}";

            _mongoClient = new MongoClient(connString);
            var mongoDb = _mongoClient.GetDatabase(_databaseName);
            
            this.DbContext = new MongoDbContext(_mongoClient, mongoDb);
        }

        public void Dispose()
        {
            _mongoClient.DropDatabase(_databaseName);
        }
    }
}
