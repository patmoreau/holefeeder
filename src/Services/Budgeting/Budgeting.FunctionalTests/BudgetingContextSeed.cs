using System;
using System.Collections.Generic;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas;
using DrifterApps.Holefeeder.Framework.SeedWork;

using MongoDB.Driver;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public static class BudgetingContextSeed
    {
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();

        public static readonly Guid AccountGuid1 = Guid.NewGuid();
        public static readonly Guid AccountGuid2 = Guid.NewGuid();
        public static readonly Guid AccountGuid3 = Guid.NewGuid();
        public static readonly Guid AccountGuid4 = Guid.NewGuid();

        public static readonly Guid CategoryGuid1 = Guid.NewGuid();
        public static readonly Guid CategoryGuid2 = Guid.NewGuid();
        public static readonly Guid CategoryGuid3 = Guid.NewGuid();

        public static readonly Guid CashflowGuid1 = Guid.NewGuid();

        public static readonly Guid TransactionGuid1 = Guid.NewGuid();
        public static readonly Guid TransactionGuid2 = Guid.NewGuid();
        public static readonly Guid TransactionGuid3 = Guid.NewGuid();
        public static readonly Guid TransactionGuid4 = Guid.NewGuid();
        public static readonly Guid TransactionGuid5 = Guid.NewGuid();
        public static readonly Guid TransactionGuid6 = Guid.NewGuid();
        public static readonly Guid TransactionGuid7 = Guid.NewGuid();

        private static readonly Dictionary<Guid, string> MongoIds = new Dictionary<Guid, string>();
        public static void SeedData(IHolefeederDatabaseSettings settings)
        {
            settings.ThrowIfNull(nameof(settings));

            var client = new MongoClient(settings.ConnectionString);

            CleanupData(client, settings);

            var database = client.GetDatabase(settings.Database);

            var accounts = database.GetCollection<AccountSchema>(AccountSchema.SCHEMA);
            CreateAccount(accounts, AccountGuid1, 1, TestUserGuid1, AccountType.Checking, false, false);
            CreateAccount(accounts, AccountGuid2, 2, TestUserGuid1, AccountType.CreditCard, true, false);
            CreateAccount(accounts, AccountGuid3, 3, TestUserGuid1, AccountType.Loan, false, true);
            CreateAccount(accounts, AccountGuid4, 4, TestUserGuid2, AccountType.Checking, false, false);
            
            var categories = database.GetCollection<CategorySchema>(CategorySchema.SCHEMA);
            CreateCategory(categories, CategoryGuid1, 1, TestUserGuid1, CategoryType.Expense, false);
            CreateCategory(categories, CategoryGuid2, 2, TestUserGuid1, CategoryType.Gain, true);
            CreateCategory(categories, CategoryGuid3, 3, TestUserGuid2, CategoryType.Expense, false);
            
            var cashflows = database.GetCollection<CashflowSchema>(CashflowSchema.SCHEMA);
            CreateCashflow(cashflows, CashflowGuid1, 1, AccountGuid1, CategoryGuid1, TestUserGuid1, 111, DateIntervalType.Weekly, 2);

            var transactions = database.GetCollection<TransactionSchema>(TransactionSchema.SCHEMA);
            CreateTransaction(transactions, TransactionGuid1, 1, AccountGuid1, CategoryGuid1, TestUserGuid1, 10);
            CreateTransaction(transactions, TransactionGuid2, 2, AccountGuid1, CategoryGuid1, TestUserGuid1, 20);
            CreateTransaction(transactions, TransactionGuid3, 3, AccountGuid1, CategoryGuid1, TestUserGuid1, 30);
            CreateTransaction(transactions, TransactionGuid4, 4, AccountGuid1, CategoryGuid2, TestUserGuid1, 40);
            CreateTransaction(transactions, TransactionGuid5, 5, AccountGuid1, CategoryGuid2, TestUserGuid1, 50);
            CreateTransaction(transactions, TransactionGuid6, 6, AccountGuid1, CategoryGuid3, TestUserGuid2, 10);
            CreateTransaction(transactions, TransactionGuid7, 7, AccountGuid1, CategoryGuid3, TestUserGuid2, 20);
        }

        private static void CreateAccount(IMongoCollection<AccountSchema> collection, Guid id, int index,
            Guid userId, AccountType type, bool favorite, bool inactive)
        {
            var schema = new AccountSchema
            {
                Id = id,
                Name = $"Account{index}",
                Description = $"Description{index}",
                Type = type,
                UserId = userId,
                Favorite = favorite,
                Inactive = inactive,
                OpenBalance = (Convert.ToDecimal(index) * 100m) + (Convert.ToDecimal(index) / 100m),
                OpenDate = (new DateTime(2019, 1, 1)).AddDays(index),
            };
            
            collection.InsertOne(schema);
            
            MongoIds.Add(id, schema.MongoId);
        }

        private static void CreateCategory(IMongoCollection<CategorySchema> collection, Guid id, int index,
            Guid userId, CategoryType type, bool favorite)
        {
            var schema = new CategorySchema
            {
                Id = id,
                Name = $"Category{index}",
                Type = type,
                UserId = userId,
                Favorite = favorite,
            };
            
            collection.InsertOne(schema);
            
            MongoIds.Add(id, schema.MongoId);
        }

        private static void CreateCashflow(IMongoCollection<CashflowSchema> collection, Guid id, int index,
            Guid accountId, Guid categoryId, Guid userId, decimal amount, DateIntervalType intervalType, int frequency)
        {
            var schema = new CashflowSchema
            {
                Id = id,
                Amount = amount,
                EffectiveDate = (new DateTime(2020, 1, 1)).AddDays(index),
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = 0,
                Description = $"Cashflow{index}",
                Account = MongoIds[accountId],
                Category = MongoIds[categoryId],
                UserId = userId
            };
            
            collection.InsertOne(schema);
            
            MongoIds.Add(id, schema.MongoId);
        }

        private static void CreateTransaction(IMongoCollection<TransactionSchema> collection, Guid id, int index,
            Guid accountId, Guid categoryId, Guid userId, decimal amount)
        {
            var schema = new TransactionSchema
            {
                Id = id,
                Amount = amount,
                Date = (new DateTime(2020, 1, 1)).AddDays(index),
                Description = $"Transaction{index}",
                Account = MongoIds[accountId],
                Category = MongoIds[categoryId],
                UserId = userId
            };
            
            collection.InsertOne(schema);
            
            MongoIds.Add(id, schema.MongoId);
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
