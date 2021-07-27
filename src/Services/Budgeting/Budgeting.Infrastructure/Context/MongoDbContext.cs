using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers;

using Framework.Dapper.SeedWork.Extensions;

using Microsoft.Extensions.Logging;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly bool _migrate;
        private readonly IHolefeederContext _sqlContext;
        private readonly ILogger<MongoDbContext> _logger;
        private readonly IMongoDatabase _database;

        private readonly IMongoClient _mongoClient;
        private readonly List<Func<Task>> _commands;

        private IClientSessionHandle _session;

        static MongoDbContext()
        {
#pragma warning disable 618
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore 618
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            BsonSerializer.RegisterSerializer(typeof(AccountType), new AccountTypeSerializer());
            BsonSerializer.RegisterSerializer(typeof(CategoryType), new CategoryTypeSerializer());
            BsonSerializer.RegisterSerializer(typeof(DateIntervalType), new DateIntervalTypeSerializer());
            BsonClassMap.RegisterClassMap<AccountSchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<CategorySchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<TransactionSchema>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        public MongoDbContext(MongoDatabaseSettings settings, IHolefeederContext sqlContext, ILogger<MongoDbContext> logger)
        {
            _sqlContext = sqlContext;
            _logger = logger;

            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _migrate = settings.Migrate;
            _mongoClient = new MongoClient(settings.ConnectionString);
            _database = _mongoClient.GetDatabase(settings.Database);

            _commands = new List<Func<Task>>();
        }

        public IMongoCollection<AccountSchema> Accounts =>
            _database.GetCollection<AccountSchema>(AccountSchema.SCHEMA);

        public IMongoCollection<CategorySchema> Categories =>
            _database.GetCollection<CategorySchema>(CategorySchema.SCHEMA);

        public IMongoCollection<CashflowSchema> Cashflows =>
            _database.GetCollection<CashflowSchema>(CashflowSchema.SCHEMA);

        public IMongoCollection<TransactionSchema> Transactions =>
            _database.GetCollection<TransactionSchema>(TransactionSchema.SCHEMA);

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            using (_session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken))
            {
                _session.StartTransaction();

                var commandTasks = _commands.Select(c => c()).ToList();

                await Task.WhenAll(commandTasks);

                commandTasks.Clear();

                await _session.CommitTransactionAsync(cancellationToken);

                return commandTasks.Count;
            }
        }

        public void ClearCommands()
        {
            _commands.Clear();
        }

        public void Migrate()
        {
            if (_migrate)
            {
                MigrateCategories();
                MigrateAccounts();
                MigrateCashflows();
                MigrateTransactions();
            }
        }

        private void MigrateAccounts()
        {
            _logger.LogInformation("Starting accounts migration");
            foreach (var schema in Accounts.AsQueryable())
            {
                var entity = _sqlContext.Connection.FindByIdAsync<AccountEntity>(new { schema.Id, schema.UserId }).Result;
                if (entity is not null)
                {
                    continue;
                }

                _sqlContext.Connection.InsertAsync(new AccountEntity
                {
                    Id = schema.Id,
                    Name = schema.Name,
                    Type = schema.Type,
                    OpenBalance = schema.OpenBalance,
                    OpenDate = schema.OpenDate,
                    Favorite = schema.Favorite,
                    Description = schema.Description,
                    Inactive = schema.Inactive,
                    UserId = schema.UserId
                }).Wait();
            }
            _logger.LogInformation("Ending accounts migration");
        }

        private void MigrateCashflows()
        {
            _logger.LogInformation("Starting cashflows migration");
            foreach (var schema in Cashflows.AsQueryable())
            {
                var entity = _sqlContext.Connection.FindByIdAsync<CashflowEntity>(new { schema.Id, schema.UserId }).Result;
                if (entity is not null)
                {
                    continue;
                }

                _sqlContext.Connection.InsertAsync(new CashflowEntity
                {
                    Id = schema.Id,
                    EffectiveDate = schema.EffectiveDate,
                    Amount = schema.Amount,
                    IntervalType = schema.IntervalType,
                    Frequency = schema.Frequency,
                    Recurrence = schema.Recurrence,
                    Description = schema.Description,
                    AccountId = Accounts.AsQueryable().Single(x => x.MongoId == schema.Account).Id,
                    CategoryId = Categories.AsQueryable().Single(x => x.MongoId == schema.Category).Id,
                    Inactive = schema.Inactive,
                    Tags = schema.Tags.ToArray(),
                    UserId = schema.UserId
                }).Wait();
            }
            _logger.LogInformation("Ending cashflows migration");
        }

        private void MigrateTransactions()
        {
            _logger.LogInformation("Starting transactions migration");
            foreach (var schema in Transactions.AsQueryable())
            {
                var entity = _sqlContext.Connection.FindByIdAsync<TransactionEntity>(new { schema.Id, schema.UserId }).Result;
                if (entity is not null)
                {
                    continue;
                }

                _sqlContext.Connection.InsertAsync(new TransactionEntity
                {
                    Id = schema.Id,
                    Date = schema.Date,
                    Amount = schema.Amount,
                    Description = schema.Description,
                    AccountId = Accounts.AsQueryable().Single(x => x.MongoId == schema.Account).Id,
                    CategoryId = Categories.AsQueryable().Single(x => x.MongoId == schema.Category).Id,
                    CashflowId = string.IsNullOrEmpty(schema.Cashflow) ? null : Cashflows.AsQueryable().Single(x => x.MongoId == schema.Cashflow).Id,
                    CashflowDate = schema.CashflowDate,
                    Tags = schema.Tags?.ToArray(),
                    UserId = schema.UserId
                }).Wait();
            }
            _logger.LogInformation("Ending transactions migration");
        }

        private void MigrateCategories()
        {
            _logger.LogInformation("Starting categories migration");
            foreach (var schema in Categories.AsQueryable())
            {
                var entity = _sqlContext.Connection.FindByIdAsync<CategoryEntity>(new { schema.Id, schema.UserId }).Result;
                if (entity is not null)
                {
                    continue;
                }

                _sqlContext.Connection.InsertAsync(new CategoryEntity
                {
                    Id = schema.Id,
                    Name = schema.Name,
                    Type = schema.Type,
                    Color = schema.Color,
                    BudgetAmount = schema.BudgetAmount,
                    Favorite = schema.Favorite,
                    System = schema.System,
                    UserId = schema.UserId
                }).Wait();
            }
            _logger.LogInformation("Ending categories migration");
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
                _session?.Dispose();
            }

            _isDisposed = true;
        }
    }
}
