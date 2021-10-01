using System;
using System.Data;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;

using Framework.Dapper.SeedWork.Extensions;

using MySqlConnector;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests
{
    public static class BudgetingContextSeed
    {
        public static readonly Guid TestUserGuid1 = Guid.NewGuid();
        public static readonly Guid TestUserGuid2 = Guid.NewGuid();
        public static readonly Guid TestUserForCommands = Guid.NewGuid();

        public static readonly Guid Account1 = Guid.NewGuid();
        public static readonly Guid Account2 = Guid.NewGuid();
        public static readonly Guid Account3 = Guid.NewGuid();
        public static readonly Guid Account4 = Guid.NewGuid();
        public static readonly Guid OpenAccountNoCashflows = Guid.NewGuid();
        public static readonly Guid OpenAccountWithCashflows = Guid.NewGuid();
        public static readonly Guid CloseAccount = Guid.NewGuid();
        public static readonly Guid TargetAccount1 = Guid.NewGuid();
        public static readonly Guid TargetAccount2 = Guid.NewGuid();

        public static readonly Guid Category1 = Guid.NewGuid();
        public static readonly Guid Category2 = Guid.NewGuid();
        public static readonly Guid Category3 = Guid.NewGuid();
        public static readonly Guid Category4 = Guid.NewGuid();
        public static readonly Guid CategoryIn = Guid.NewGuid();
        public static readonly Guid CategoryOut = Guid.NewGuid();

        public static readonly Guid Cashflow1 = Guid.NewGuid();
        public static readonly Guid Cashflow2 = Guid.NewGuid();
        public static readonly Guid Cashflow3 = Guid.NewGuid();
        public static readonly Guid Cashflow4 = Guid.NewGuid();

        public static readonly Guid Transaction1 = Guid.NewGuid();
        public static readonly Guid Transaction2 = Guid.NewGuid();
        public static readonly Guid Transaction3 = Guid.NewGuid();
        public static readonly Guid Transaction4 = Guid.NewGuid();
        public static readonly Guid Transaction5 = Guid.NewGuid();
        public static readonly Guid Transaction6 = Guid.NewGuid();
        public static readonly Guid Transaction7 = Guid.NewGuid();
        public static readonly Guid Transaction8 = Guid.NewGuid();

        public static void PrepareData(HolefeederDatabaseSettings settings)
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

        public static void SeedData(HolefeederDatabaseSettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            using (var connection = new MySqlConnection(settings.ConnectionString))
            {
                connection.Open();

                AccountBuilder.Create(Account1).OfType(AccountType.Checking).ForUser(TestUserGuid1).Build();
                AccountBuilder.Create(Account2).OfType(AccountType.CreditCard).ForUser(TestUserGuid1).IsFavorite()
                    .Build();
                AccountBuilder.Create(Account3).OfType(AccountType.Loan).ForUser(TestUserGuid1).IsInactive().Build();
                AccountBuilder.Create(Account4).OfType(AccountType.Checking).ForUser(TestUserGuid2).Build();
                AccountBuilder.Create(OpenAccountNoCashflows).OfType(AccountType.Checking).ForUser(TestUserForCommands)
                    .Build();
                AccountBuilder.Create(OpenAccountWithCashflows).OfType(AccountType.Checking)
                    .ForUser(TestUserForCommands).Build();
                AccountBuilder.Create(TargetAccount1).OfType(AccountType.Checking)
                    .ForUser(TestUserForCommands).Build();
                AccountBuilder.Create(TargetAccount2).OfType(AccountType.Savings)
                    .ForUser(TestUserForCommands).Build();
                foreach (var entity in AccountBuilder.Accounts)
                {
                    connection.InsertAsync(entity).Wait();
                }

                CategoryBuilder.Create(Category1).OfType(CategoryType.Expense).ForUser(TestUserGuid1).Build();
                CategoryBuilder.Create(Category2).OfType(CategoryType.Gain).ForUser(TestUserGuid1).IsFavorite().Build();
                CategoryBuilder.Create(Category3).OfType(CategoryType.Expense).ForUser(TestUserGuid2).Build();
                CategoryBuilder.Create(Category4).OfType(CategoryType.Expense).ForUser(TestUserForCommands).Build();
                CategoryBuilder.Create(CategoryIn).Named("Transfer In").OfType(CategoryType.Gain).ForUser(TestUserForCommands).Build();
                CategoryBuilder.Create(CategoryOut).Named("Transfer Out").OfType(CategoryType.Expense).ForUser(TestUserForCommands).Build();
                foreach (var entity in CategoryBuilder.Categories)
                {
                    connection.InsertAsync(entity).Wait();
                }

                CashflowBuilder.Create(Cashflow1).OfType(DateIntervalType.Weekly).WithFrequency(2).OfAmount(111)
                    .ForAccount(Account1).ForCategory(Category1).ForUser(TestUserGuid1).Build();
                CashflowBuilder.Create(Cashflow2).OfType(DateIntervalType.Weekly).WithFrequency(2).OfAmount(111)
                    .ForAccount(OpenAccountWithCashflows).ForCategory(Category4).ForUser(TestUserForCommands).Build();
                CashflowBuilder.Create(Cashflow3).OfType(DateIntervalType.Weekly).WithFrequency(2).OfAmount(222)
                    .ForAccount(Account4).ForCategory(Category1).ForUser(TestUserGuid2).Build();
                CashflowBuilder.Create(Cashflow4).OfType(DateIntervalType.OneTime).OfAmount(333)
                    .ForAccount(Account4).ForCategory(Category1).ForUser(TestUserGuid2).Build();
                foreach (var entity in CashflowBuilder.Cashflows)
                {
                    connection.InsertAsync(entity).Wait();
                }

                TransactionBuilder.Create(Transaction1).OfAmount(10).ForAccount(Account1).ForCategory(Category1)
                    .ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction2).OfAmount(20).ForAccount(Account1).ForCategory(Category1)
                    .ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction3).OfAmount(30).ForAccount(Account1).ForCategory(Category1)
                    .ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction4).OfAmount(40).ForAccount(Account1).ForCategory(Category2)
                    .ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction5).OfAmount(50).ForAccount(Account1).ForCategory(Category2)
                    .ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction6).OfAmount(111).IsCashflow(Cashflow1, new DateTime(2020, 1, 2))
                    .ForAccount(Account1).ForCategory(Category1).ForUser(TestUserGuid1).Build();
                TransactionBuilder.Create(Transaction7).OfAmount(10).ForAccount(Account4).ForCategory(Category3)
                    .ForUser(TestUserGuid2).Build();
                TransactionBuilder.Create(Transaction8).OfAmount(20).ForAccount(Account4).ForCategory(Category3)
                    .ForUser(TestUserGuid2).Build();
                foreach (var entity in TransactionBuilder.Transactions)
                {
                    connection.InsertAsync(entity).Wait();
                }

                connection.Close();
            }
        }

        private static void RefreshDb(IDbConnection connection, string databaseName)
        {
            connection.Execute($"DROP DATABASE IF EXISTS {databaseName};");
        }
    }
}
