using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;
using DrifterApps.Holefeeder.ObjectStore.API;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

using Xbehave;

using Xunit;

namespace ObjectStore.FunctionalTests.Scenarios
{
    public class ObjectStoreScenarios : IClassFixture<ObjectStoreWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true,};

        public ObjectStoreScenarios(ObjectStoreWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Scenario]
        public void GetStoreItems(HttpClient client, HttpResponseMessage response)
        {
            "Given get store items"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "When I get call the API"
                .x(async () =>
                {
                    const string requestUri = "/api/v2/StoreItems/";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the store items of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"),
                        new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"),
                        new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code3", "Data3")
                    );
                });
        }

        [Scenario]
        public void GetStoreItems_WithFilter(HttpClient client, string filter, HttpResponseMessage response)
        {
            "Given get store items"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "With filter code=Code2"
                .x(() => filter = "code=Code2");

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems?filter={filter}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should()
                        .BeEquivalentTo(new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"));
                });
        }

        [Scenario]
        public void GetStoreItems_WithQueryParams(HttpClient client, string sort, int offset, int limit,
            HttpResponseMessage response)
        {
            "Given get store items"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "Sorted on code descending"
                .x(() => sort = "-code");

            "With an offset of 1"
                .x(() => offset = 1);

            "And a limit of 2"
                .x(() => limit = 2);

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems?sort={sort}&offset={offset}&limit={limit}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"),
                        new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"));
                });
        }

        [Scenario]
        public void GetStoreItem(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given get store item"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "With Id #1"
                .x(() => id = StoreItemContextSeed.Guid3);

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate success"
                .x(() => response.StatusCodeShouldBeSuccess());

            "And the result contain the store item Code2 of the user"
                .x(async () =>
                {
                    var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel>(_jsonSerializerOptions);

                    result.Should().BeEquivalentTo(
                        new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code3", "Data3"));
                });
        }

        [Scenario]
        public void GetStoreItem_WithInvalidId(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given get store item"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "With invalid Id"
                .x(() => id = Guid.Empty);

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate BadRequest"
                .x(() => response.StatusCode.Should().Be(StatusCodes.Status400BadRequest));
        }

        [Scenario]
        public void GetStoreItem_WhenIdDoesntExists(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given get store item"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "With non existent Id"
                .x(() => id = Guid.NewGuid());

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate NotFound"
                .x(() => response.StatusCode.Should().Be(StatusCodes.Status404NotFound));
        }

        [Scenario]
        public void GetStoreItem_WithIdFromAnotherUser(HttpClient client, Guid id, HttpResponseMessage response)
        {
            "Given get store item"
                .x(() => client = _factory.CreateDefaultClient());

            "For user TestUser #1"
                .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                    StoreItemContextSeed.TestUserGuid1.ToString()));

            "With item Id for TestUser #2"
                .x(() => id = StoreItemContextSeed.Guid4);

            "When I get call the API"
                .x(async () =>
                {
                    var requestUri = $"/api/v2/StoreItems/{id.ToString()}";

                    response = await client.GetAsync(requestUri);
                });

            "Then the status code should indicate NotFound"
                .x(() => response.StatusCode.Should().Be(StatusCodes.Status404NotFound));
        }

        [Fact]
        public async Task GivenCreateObjectCommand_WhenCommandValid_ThenObjectCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER, Guid.NewGuid().ToString());
            var request = $"/api/v2/StoreItems/create-store-item";

            // Act
            var command = new CreateStoreItemCommand {Code = "New Code", Data = "New Data"};
            var response = await client.PostAsJsonAsync(request, command);

            // Assert
            response.StatusCodeShouldBeSuccess();
            var result = await response.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);

            result.Should().NotBeNull();
            response.Headers.Location?.AbsolutePath.Should().BeEquivalentTo($"/api/v2/StoreItems/create-store-item");
            result.Status.Should().Be(CommandStatus.Created);
            result.Result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GivenCreateObjectCommand_WhenCommandInvalid_ThenReturnError()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = $"/api/v2/StoreItems/create-store-item";

            // Act
            var response = await client.PostAsJsonAsync(request, new { });

            // Assert
            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task GivenCreateObjectCommand_WhenCodeAlreadyExist_ThenReturnError()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = $"/api/v2/StoreItems/create-store-item";

            // Act
            var command = new CreateStoreItemCommand {Code = "Code1", Data = "Data1"};
            var response = await client.PostAsJsonAsync(request, command);

            var result = await response.Content.ReadFromJsonAsync<CommandResult>(_jsonSerializerOptions);

            // Assert
            result.Should().BeEquivalentTo(
                CommandResult.Create(CommandStatus.BadRequest, "Code 'Code1' already exists."),
                options => options.ComparingByMembers<CommandResult>());
        }

        [Fact]
        public async Task GivenModifyObjectCommand_WhenCommandValid_ThenObjectModified()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER, Guid.NewGuid().ToString());
            var createRequest = $"/api/v2/StoreItems/create-store-item";
            var modifyRequest = $"/api/v2/StoreItems/modify-store-item";

            var code = Guid.NewGuid();
            var data = Guid.NewGuid();

            // Act
            var createCommand = new CreateStoreItemCommand {Code = code.ToString(), Data = data.ToString()};
            var createResponse = await client.PostAsJsonAsync(createRequest, createCommand);

            var createResult =
                await createResponse.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);

            var modifyCommand =
                new ModifyStoreItemCommand
                {
                    Id = createResult?.Result ?? Guid.Empty, Data = $"{data.ToString()}-modified"
                };
            var modifyResponse = await client.PutAsJsonAsync(modifyRequest, modifyCommand);

            // Assert
            modifyResponse.StatusCodeShouldBeSuccess();
            var result = await modifyResponse.Content.ReadFromJsonAsync<CommandResult>(_jsonSerializerOptions);

            result.Should().NotBeNull();
            result.Status.Should().Be(CommandStatus.Ok);
        }
    }
}
