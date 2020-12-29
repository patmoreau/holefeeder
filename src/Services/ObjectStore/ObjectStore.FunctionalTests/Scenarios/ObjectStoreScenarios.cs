using System;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;
using DrifterApps.Holefeeder.ObjectStore.API;
using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using FluentAssertions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

using Xunit;

namespace ObjectStore.FunctionalTests.Scenarios
{
    public class ObjectStoreScenarios : IClassFixture<ObjectStoreWebApplicationFactory>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public ObjectStoreScenarios(ObjectStoreWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GivenGetStoreItems_WhenNoFilterApplied_ThenReturnAllItems()
        {
            // Arrange
            var client = _factory.CreateDefaultClient();
            const string request = "/api/v2/StoreItems/";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCodeShouldBeSuccess();

            var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GivenGetStoreItems_WhenFilterCode2Applied_ThenReturnItem()
        {
            // Arrange
            var client = _factory.CreateDefaultClient();
            const string request = "/api/v2/StoreItems?filter=code=Code2";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCodeShouldBeSuccess();

            var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

            result.Should().BeEquivalentTo(new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"));
        }

        [Fact]
        public async Task GivenGetStoreItems_WhenQueryParamsApplied_ThenReturnItemsInProperOrder()
        {
            // Arrange
            var client = _factory.CreateDefaultClient();
            const string request = "/api/v2/StoreItems?sort=-code&offset=1&limit=2";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCodeShouldBeSuccess();

            var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel[]>(_jsonSerializerOptions);

            result.Should()
                .BeEquivalentTo(new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code2", "Data2"),
                    new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"));
        }

        [Fact]
        public async Task GivenGetStoreItem_WhenValidId_ThenReturnItem()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = $"/api/v2/StoreItems/{StoreItemContextSeed.Guid1.ToString()}";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCodeShouldBeSuccess();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<StoreItemViewModel>(json, _jsonSerializerOptions);

            result.Should().BeEquivalentTo(new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code1", "Data1"));
        }

        [Fact]
        public async Task GivenGetStoreItem_WhenInvalidId_ThenReturnBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = $"/api/v2/StoreItems/{Guid.Empty}";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task GivenGetStoreItem_WhenIdDoesntExist_ThenReturnNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = $"/api/v2/StoreItems/{Guid.NewGuid()}";

            // Act
            var response = await client.GetAsync(request);

            // Assert
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GivenCreateObjectCommand_WhenCommandValid_ThenObjectCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER, Guid.NewGuid().ToString());
            var request = $"/api/v2/StoreItems/create-store-item";

            // Act
            var command = new CreateStoreItemCommand("New Code", "New Data");
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
            var response = await client.PostAsJsonAsync(request, new {});

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
            var command = new CreateStoreItemCommand("Code1", "Data1");
            var response = await client.PostAsJsonAsync(request, command);

            // Assert
            var result = await response.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);

            result.Should().BeEquivalentTo(
                new CommandResult<Guid>(CommandStatus.BadRequest, Guid.Empty, $"Code 'Code1' already exists."));
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
            var createCommand = new CreateStoreItemCommand(code.ToString(), data.ToString());
            var createResponse = await client.PostAsJsonAsync(createRequest, createCommand);

            var createResult = await createResponse.Content.ReadFromJsonAsync<CommandResult<Guid>>(_jsonSerializerOptions);

            var modifyCommand = new ModifyStoreItemCommand(createResult?.Result ?? Guid.Empty, $"{data.ToString()}-modified");
            var modifyResponse = await client.PutAsJsonAsync(modifyRequest, modifyCommand);

            // Assert
            modifyResponse.StatusCodeShouldBeSuccess();
            var result = await modifyResponse.Content.ReadFromJsonAsync<CommandResult<Unit>>(_jsonSerializerOptions);

            result.Should().NotBeNull();
            result.Status.Should().Be(CommandStatus.Ok);
        }
    }
}
