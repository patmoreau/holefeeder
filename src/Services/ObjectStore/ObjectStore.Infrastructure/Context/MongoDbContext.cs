using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        private readonly IMongoClient _mongoClient;
        private readonly List<Func<Task>> _commands;

        private IClientSessionHandle session;

        static MongoDbContext()
        {
#pragma warning disable 618
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore 618
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonClassMap.RegisterClassMap<StoreItemSchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        public MongoDbContext(IHolefeederDatabaseSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));
            
            _mongoClient = new MongoClient(settings.ConnectionString);
            _database = _mongoClient.GetDatabase(settings.Database);

            _commands = new List<Func<Task>>();
        }

        public async Task<IMongoCollection<StoreItemSchema>> GetStoreItemsAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = _database.GetCollection<StoreItemSchema>(StoreItemSchema.SCHEMA);

            var indexes = await collection.Indexes.ListAsync(cancellationToken);
            var hasIndexes = await indexes.AnyAsync(cancellationToken);
            if (hasIndexes)
            {
                return collection;
            }

            var indexBuilder = Builders<StoreItemSchema>.IndexKeys;
            var keys = indexBuilder.Ascending(a => a.Id);
            var options = new CreateIndexOptions {Background = true, Unique = true};
            var indexModel = new CreateIndexModel<StoreItemSchema>(keys, options);
            await collection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);

            return collection;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            using (session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken))
            {
                var transactionOptions = new TransactionOptions(
                    readPreference: ReadPreference.Primary,
                    readConcern: ReadConcern.Local,
                    writeConcern: WriteConcern.WMajority);

                var result = await session.WithTransaction(
                    async (s, ct) =>
                    {
                        var commandTasks = _commands.Select(c => c()).ToList();

                        await Task.WhenAll(commandTasks);

                        _commands.Clear();
                        
                        return commandTasks.Count;
                    },
                    transactionOptions,
                    cancellationToken);
                return result;
            }
        }

        public void ClearCommands()
        {
            _commands.Clear();
        }

        private bool _isDisposed;
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                ClearCommands();
                session?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
