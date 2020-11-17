using System;
using System.Diagnostics;
using DrifterApps.Holefeeder.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using DrifterApps.Holefeeder.Infrastructure.Database.Schemas;
using FluentAssertions;
using MongoDB.Bson;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Tests
{
    public static class GenerateTestData
    {
        public static (ObjectId MongoId, Guid Id) CreateAccountSchema(
            this IMongoDbContext context,
            int index,
            AccountType type,
            Guid userId,
            bool favorite = false,
            bool inactive = false)
        {
            _ = context.ThrowIfNull(nameof(context));

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetAccountsAsync().Result.InsertOne(new AccountSchema
            {
                MongoId = mongoId.ToString(),
                Name = $"Account{index}",
                Type = type,
                Description = $"Account #{index}",
                Id = id,
                Favorite = favorite,
                Inactive = inactive,
                OpenBalance = (Convert.ToDecimal(index) * 100m) + (Convert.ToDecimal(index) / 100m),
                OpenDate = (new DateTime(2019, 1, 1)).AddDays(index),
                UserId = userId
            });
            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateCategorySchema(
            this IMongoDbContext context,
            int index,
            CategoryType type,
            Guid userId,
            bool favorite = false)
        {
            _ = context.ThrowIfNull(nameof(context));

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetCategoriesAsync().Result.InsertOne(new CategorySchema
            {
                MongoId = mongoId.ToString(),
                Name = $"Category{index}",
                Type = type,
                Id = id,
                Favorite = favorite,
                UserId = userId
            });

            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateCashflowSchema(
            this IMongoDbContext context,
            int index,
            (ObjectId MongoId, Guid Id) account,
            (ObjectId MongoId, Guid Id) category,
            decimal amount,
            DateIntervalType intervalType,
            int frequency,
            Guid userId)
        {
            _ = context.ThrowIfNull(nameof(context));

            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetCashflowsAsync().Result.InsertOne(new CashflowSchema
            {
                MongoId = mongoId.ToString(),
                Amount = amount,
                EffectiveDate = (new DateTime(2020, 1, 1)).AddDays(index),
                IntervalType = intervalType,
                Frequency = frequency,
                Recurrence = 0,
                Description = $"Transaction{index}",
                Account = account.MongoId.ToString(),
                Category = category.MongoId.ToString(),
                Id = id,
                UserId = userId
            });

            return (mongoId, id);
        }

        public static (ObjectId MongoId, Guid Id) CreateTransactionSchema(
            this IMongoDbContext context,
            int index,
            (ObjectId MongoId, Guid Id) account,
            (ObjectId MongoId, Guid Id) category,
            decimal amount,
            Guid userId
        )
        {
            context.ThrowIfNull(nameof(context));
            
            var mongoId = ObjectId.GenerateNewId();
            var id = Guid.NewGuid();

            context.GetTransactionsAsync().Result.InsertOne(new TransactionSchema
            {
                MongoId = mongoId.ToString(),
                Amount = amount,
                Date = (new DateTime(2020, 1, 1)).AddDays(index),
                Description = $"Transaction{index}",
                Account = account.MongoId.ToString(),
                Category = category.MongoId.ToString(),
                Id = id,
                UserId = userId
            });

            return (mongoId, id);
        }
    }
}
