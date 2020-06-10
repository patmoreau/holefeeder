using System;
using System.Linq;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Application.SeedWork;
using DrifterApps.Holefeeder.Domain.Enumerations;
using DrifterApps.Holefeeder.Infrastructure.Database.Repositories;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Tests.Repositories
{
    public class AccountQueriesRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public AccountQueriesRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        private void InitBaseData(string testName)
        {
            _testUsers ??= new[]
            {
                _fixture.DatabaseContext.CreateTestUserSchema($"{testName}#1"),
                _fixture.DatabaseContext.CreateTestUserSchema($"{testName}#2")
            };
            _accounts ??= new[]
            {
                _fixture.DatabaseContext.CreateAccountSchema(1, AccountType.Checking, _testUsers[0]),
                _fixture.DatabaseContext.CreateAccountSchema(2, AccountType.CreditCard, _testUsers[0], true),
                _fixture.DatabaseContext.CreateAccountSchema(3, AccountType.Loan, _testUsers[0], inactive: true),
                _fixture.DatabaseContext.CreateAccountSchema(4, AccountType.Checking, _testUsers[1])
            };
            _categories = new[]
            {
                _fixture.DatabaseContext.CreateCategorySchema(1, CategoryType.Expense, _testUsers[0]),
                _fixture.DatabaseContext.CreateCategorySchema(2, CategoryType.Gain, _testUsers[0]),
                _fixture.DatabaseContext.CreateCategorySchema(3, CategoryType.Expense, _testUsers[1])
            };
            _transactions = new[]
            {
                _fixture.DatabaseContext.CreateTransactionSchema(1, _accounts[0], _categories[0], 10, _testUsers[0]),
                _fixture.DatabaseContext.CreateTransactionSchema(2, _accounts[0], _categories[0], 20, _testUsers[0]),
                _fixture.DatabaseContext.CreateTransactionSchema(3, _accounts[0], _categories[0], 30, _testUsers[0]),
                _fixture.DatabaseContext.CreateTransactionSchema(4, _accounts[0], _categories[1], 40, _testUsers[0]),
                _fixture.DatabaseContext.CreateTransactionSchema(5, _accounts[0], _categories[1], 50, _testUsers[0]),
                _fixture.DatabaseContext.CreateTransactionSchema(6, _accounts[3], _categories[2], 10, _testUsers[1]),
                _fixture.DatabaseContext.CreateTransactionSchema(7, _accounts[3], _categories[2], 20, _testUsers[1])
            };
        }

        private (ObjectId MongoId, Guid Id)[] _testUsers;
        private (ObjectId MongoId, Guid Id)[] _accounts;
        private (ObjectId MongoId, Guid Id)[] _categories;
        // ReSharper disable once NotAccessedField.Local
        private (ObjectId MongoId, Guid Id)[] _transactions;

        [Fact]
        public async void GivenGetAccounts_WhenNoFilter_ThenReturnResultsForUser()
        {
            InitBaseData(nameof(GivenGetAccounts_WhenNoFilter_ThenReturnResultsForUser));

            var repository = new AccountQueriesRepository(_fixture.DatabaseContext);
            var result = await repository.GetAccountsAsync(_testUsers[0].Id, QueryParams.Empty, default);

            result.Should().BeEquivalentTo(
                new AccountViewModel(_accounts[0].Id, AccountType.Checking, "Account1",
                    5, 130.01m, (new DateTime(2020, 1, 1)).AddDays(5), "Account #1", false),
                new AccountViewModel(_accounts[1].Id, AccountType.CreditCard,
                    "Account2", 0, 200.02m, (new DateTime(2019, 1, 1)).AddDays(2), "Account #2", true),
                new AccountViewModel(_accounts[2].Id, AccountType.Loan, "Account3", 0,
                    300.03m, (new DateTime(2019, 1, 1)).AddDays(3), "Account #3", false)
            );
        }

        [Fact]
        public async void GivenGetAccounts_WhenFilteredOnAccount1_ThenReturnAccount1()
        {
            InitBaseData(nameof(GivenGetAccounts_WhenFilteredOnAccount1_ThenReturnAccount1));

            var repository = new AccountQueriesRepository(_fixture.DatabaseContext);
            var result =
                await repository.GetAccountsAsync(_testUsers[0].Id,
                    new QueryParams(0, int.MaxValue, null, new[] {"name=Account1"}),
                    default);

            result.Should().BeEquivalentTo(
                new AccountViewModel(_accounts[0].Id, AccountType.Checking, "Account1",
                    5, 130.01m, (new DateTime(2020, 1, 1)).AddDays(5), "Account #1", false)
            );
        }

        [Fact]
        public async void GivenGetAccounts_WhenUserNotExists_ThenReturnEmptyList()
        {
            InitBaseData(nameof(GivenGetAccounts_WhenUserNotExists_ThenReturnEmptyList));

            var repository = new AccountQueriesRepository(_fixture.DatabaseContext);
            var result =
                await repository.GetAccountsAsync(Guid.NewGuid(), QueryParams.Empty, default);

            result.Should().BeEmpty();
        }

        [Fact]
        public async void GivenGetAccounts_WhenOffsetAndLimitSet_ThenReturnSelectedResults()
        {
            InitBaseData(nameof(GivenGetAccounts_WhenOffsetAndLimitSet_ThenReturnSelectedResults));

            var repository = new AccountQueriesRepository(_fixture.DatabaseContext);
            var result =
                await repository.GetAccountsAsync(_testUsers[0].Id, new QueryParams(1, 2, null, null), default);

            result.Should().BeEquivalentTo(
                new AccountViewModel(_accounts[1].Id, AccountType.CreditCard,
                    "Account2", 0, 200.02m, (new DateTime(2019, 1, 1)).AddDays(2), "Account #2", true),
                new AccountViewModel(_accounts[2].Id, AccountType.Loan, "Account3", 0,
                    300.03m, (new DateTime(2019, 1, 1)).AddDays(3), "Account #3", false)
            );
        }

        [Fact]
        public async void GivenGetAccounts_WhenSorted_ThenReturnSortedResults()
        {
            InitBaseData(nameof(GivenGetAccounts_WhenSorted_ThenReturnSortedResults));

            var repository = new AccountQueriesRepository(_fixture.DatabaseContext);
            var result = await repository.GetAccountsAsync(_testUsers[0].Id,
                new QueryParams(0, int.MaxValue, new[] {"-favorite", "balance"}, null), default);

            result.Select(x => x.Id).Should().ContainInOrder(_accounts[1].Id, _accounts[0].Id, _accounts[2].Id);
        }
    }
}
