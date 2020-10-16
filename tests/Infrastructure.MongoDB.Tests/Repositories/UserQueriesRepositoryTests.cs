using System;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Schemas;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Tests.Context;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Tests.Repositories
{
    public class UserQueriesRepositoryTests : IClassFixture<DbFixture>
    {
        private readonly DbFixture _fixture;

        public UserQueriesRepositoryTests(DbFixture fixture)
        {
            _fixture = fixture;
        }

        private void InitBaseData(string testName)
        {
            _testUsers ??= new[]
            {
                _fixture.DbContext.CreateTestUserSchema($"{testName}#1"),
                _fixture.DbContext.CreateTestUserSchema($"{testName}#2")
            };
        }

        private (ObjectId MongoId, Guid Id)[] _testUsers;

        [Fact]
        public async void GivenGetUser_WhenValidEmail_ThenReturnUser()
        {
            InitBaseData(nameof(GivenGetUser_WhenValidEmail_ThenReturnUser));

            var repository = new UserQueriesRepository(_fixture.DbContext);
            var result = await repository.GetUserByEmailAsync($"{_testUsers[1].Id.ToString()}@email.com");

            result.Should().BeEquivalentTo(
                new UserViewModel(_testUsers[1].Id, "TestUser",
                    $"{nameof(GivenGetUser_WhenValidEmail_ThenReturnUser)}#2",
                    $"{_testUsers[1].Id.ToString()}@email.com", $"GoogleId{_testUsers[1].Id.ToString()}",
                    DateTime.Today));
        }

        [Fact]
        public async void GivenGetUser_WhenEmailNotFound_ThenReturnNull()
        {
            InitBaseData(nameof(GivenGetUser_WhenEmailNotFound_ThenReturnNull));

            var repository = new UserQueriesRepository(_fixture.DbContext);
            var result = await repository.GetUserByEmailAsync("not_found@email.com");

            result.Should().BeNull();
        }

        [Fact]
        public async void GivenGetUser_WhenEmailFoundButNoIdAvailable_ThenReturnUserWithNewId()
        {
            var mongoId = ObjectId.GenerateNewId();

            await _fixture.DbContext.GetUsersAsync().Result.InsertOneAsync(new UserSchema
            {
                MongoId = mongoId.ToString(),
                FirstName = "TestUser",
                LastName = nameof(GivenGetUser_WhenEmailFoundButNoIdAvailable_ThenReturnUserWithNewId),
                EmailAddress = $"{mongoId.ToString()}@email.com",
                DateJoined = DateTime.Today,
                GoogleId = $"GoogleId{mongoId.ToString()}",
                Id = Guid.Empty
            });

            var repository = new UserQueriesRepository(_fixture.DbContext);
            var result = await repository.GetUserByEmailAsync($"{mongoId.ToString()}@email.com");

            result.Should().BeEquivalentTo(
                new UserViewModel(Guid.Empty, "TestUser",
                    nameof(GivenGetUser_WhenEmailFoundButNoIdAvailable_ThenReturnUserWithNewId),
                    $"{mongoId.ToString()}@email.com", $"GoogleId{mongoId.ToString()}", DateTime.Today),
                config => config.Excluding(x => x.Id));
            result.Id.Should().NotBeEmpty();
        }
    }
}
