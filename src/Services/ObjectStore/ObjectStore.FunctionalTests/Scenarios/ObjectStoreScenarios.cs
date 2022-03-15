using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems;
using DrifterApps.Holefeeder.ObjectStore.Application.StoreItems.Commands;

using FluentAssertions;
using FluentAssertions.Execution;

using Microsoft.AspNetCore.Http;

using Xbehave;

using Xunit;

namespace ObjectStore.FunctionalTests.Scenarios;

public class ObjectStoreScenarios : IClassFixture<ObjectStoreWebApplicationFactory>
{
    private readonly ObjectStoreWebApplicationFactory _factory;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    public ObjectStoreScenarios(ObjectStoreWebApplicationFactory factory)
    {
        _factory = factory;

        _factory.SeedData();
    }

    [Scenario]
    public void GetStoreItems(HttpClient client, IEnumerable<StoreItemViewModel>? result)
    {
        "Given GetStoreItems query"
            .x(() => client = _factory.CreateDefaultClient());

        "When I call the API"
            .x(async () =>
            {
                const string requestUri = "/api/v2/store-items";

                result = await client.GetFromJsonAsync<IEnumerable<StoreItemViewModel>>(requestUri);
            });

        "Then the result contain the store items of the user"
            .x(() => result.Should()
                .NotBeNull()
                .And.BeEquivalentTo(
                    new[]
                    {
                        new StoreItemViewModel(StoreItemContextSeed.Guid1, "Code001", "Data001"),
                        new StoreItemViewModel(StoreItemContextSeed.Guid2, "Code002", "Data002"),
                        new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code003", "Data003")
                    }));
    }

    [Scenario]
    [Example("code:eq:Code057", "", QueryParams.DEFAULT_OFFSET, QueryParams.DEFAULT_LIMIT, 1, 1, "Code057",
        "Code057")]
    [Example("code:lt:Code010", "code", QueryParams.DEFAULT_OFFSET, QueryParams.DEFAULT_LIMIT, 9, 9, "Code001",
        "Code009")]
    [Example("", "-code", 1, 5, 5, 100, "Code099", "Code095")]
    [Example("", "code", 10, 30, 30, 100, "Code011", "Code040")]
    [Example("", "code", 90, 30, 10, 100, "Code091", "Code100")]
    public void GetStoreItems_WithQueryParams(string filter, string sort, int offset, int limit, int count,
        int totalCount, string first, string last, HttpClient client, IEnumerable<StoreItemViewModel>? result)
    {
        "Given GetStoreItems query"
            .x(() => client = _factory.CreateDefaultClient());

        "For user TestUser #3"
            .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                StoreItemContextSeed.TestUserGuid3.ToString()));

        "When I call the API"
            .x(async () =>
            {
                var sb = new StringBuilder("/api/v2/store-items?");
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    sb.Append($"filter={filter}&");
                }

                if (!string.IsNullOrWhiteSpace(sort))
                {
                    sb.Append($"sort={sort}&");
                }

                if (offset != QueryParams.DEFAULT_OFFSET)
                {
                    sb.Append($"offset={offset}&");
                }

                if (limit != QueryParams.DEFAULT_LIMIT)
                {
                    sb.Append($"limit={limit}&");
                }

                var requestUri = sb.ToString();

                result = await client.GetFromJsonAsync<IEnumerable<StoreItemViewModel>>(requestUri);
            });

        "And the result contains the expected query results"
            .x(() =>
            {
                using (new AssertionScope())
                {
                    result.Should()
                        .NotBeNull()
                        .And.HaveCount(count)
                        .And.StartWith(new[] {first}, (head, expected) => head.Code == expected)
                        .And.EndWith(new[] {last}, (tail, expected) => tail.Code == expected);
                }
            });
    }

    [Scenario]
    public void GetStoreItem(HttpClient client, Guid id, HttpResponseMessage response)
    {
        "Given GetStoreItem query"
            .x(() => client = _factory.CreateDefaultClient());

        "With Id #3"
            .x(() => id = StoreItemContextSeed.Guid3);

        "When I call the API"
            .x(async () =>
            {
                var requestUri = $"/api/v2/store-items/{id.ToString()}";

                response = await client.GetAsync(requestUri);
            });

        "Then the status code should indicate success"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.IsSuccessStatusCode.Should().BeTrue());

        "And the result contain the store item Code2 of the user"
            .x(async () =>
            {
                var result = await response.Content.ReadFromJsonAsync<StoreItemViewModel>(_jsonSerializerOptions);

                result.Should()
                    .NotBeNull()
                    .And.Be(new StoreItemViewModel(StoreItemContextSeed.Guid3, "Code003", "Data003"));
            });
    }

    [Scenario]
    public void GetStoreItem_WithInvalidId(HttpClient client, Guid id, HttpResponseMessage response)
    {
        "Given GetStoreItem query"
            .x(() => client = _factory.CreateDefaultClient());

        "With an invalid Id"
            .x(() => id = Guid.Empty);

        "When I call the API"
            .x(async () =>
            {
                var requestUri = $"/api/v2/store-items/{id.ToString()}";

                response = await client.GetAsync(requestUri);
            });

        "Then the status code should indicate NotFound"
            .x(() => response.StatusCode.Should().Be(HttpStatusCode.NotFound));
    }

    [Scenario]
    public void GetStoreItem_WhenIdDoesntExists(HttpClient client, Guid id, HttpResponseMessage response)
    {
        "Given GetStoreItem query"
            .x(() => client = _factory.CreateDefaultClient());

        "With non existent Id"
            .x(() => id = Guid.NewGuid());

        "When I call the API"
            .x(async () =>
            {
                var requestUri = $"/api/v2/store-items/{id.ToString()}";

                response = await client.GetAsync(requestUri);
            });

        "Then the status code should indicate NotFound"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.StatusCode.Should().Be(HttpStatusCode.NotFound));
    }

    [Scenario]
    public void GetStoreItem_WithIdFromAnotherUser(HttpClient client, Guid id, HttpResponseMessage response)
    {
        "Given GetStoreItem query"
            .x(() => client = _factory.CreateDefaultClient());

        "With item Id belonging to TestUser #2"
            .x(() => id = StoreItemContextSeed.Guid4);

        "When I call the API"
            .x(async () =>
            {
                var requestUri = $"/api/v2/store-items/{id.ToString()}";

                response = await client.GetAsync(requestUri);
            });

        "Then the status code should indicate NotFound"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.StatusCode.Should().Be(HttpStatusCode.NotFound));
    }

    [Scenario]
    public void CreateStoreItemCommand(HttpClient client, CreateStoreItem.Request command,
        HttpResponseMessage response)
    {
        "Given CreateStoreItem command"
            .x(() => client = _factory.CreateClient());

        "For newly registered test user"
            .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                Guid.NewGuid().ToString()));

        "With valid data"
            .x(() => command = new CreateStoreItem.Request("New Code", "New Data"));

        "When I call the API"
            .x(async () =>
            {
                const string requestUri = "/api/v2/store-items/create-store-item";

                response = await client.PostAsJsonAsync(requestUri, command);
            });

        "Then the status code should indicate success"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.IsSuccessStatusCode.Should().BeTrue());

        "With the header location present"
            .x(() =>
            {
                using (new AssertionScope())
                {
                    response.Headers.Location.Should().NotBeNull();
                    response.Headers.Location!.AbsolutePath.Should()
                        .MatchRegex(
                            "^/api/v2/store-items/[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$");
                }
            });

        "And a Guid is returned"
            .x(async () =>
            {
                var result =
                    await response.Content.ReadFromJsonAsync<JsonElement>(_jsonSerializerOptions);
                result.GetProperty("id").GetGuid().Should().NotBeEmpty();
            });
    }

    [Scenario]
    public void CreateObjectCommand_WithInvalidCommand(HttpClient client, HttpResponseMessage response)
    {
        "Given CreateStoreItem command"
            .x(() => client = _factory.CreateDefaultClient());

        "For newly registered test user"
            .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                Guid.NewGuid().ToString()));

        "When I call the API with invalid data"
            .x(async () =>
            {
                const string requestUri = "/api/v2/store-items/create-store-item";

                response = await client.PostAsJsonAsync(requestUri, new { });
            });

        "Then the status code should indicate UnprocessableEntity"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity));
    }

    [Scenario]
    public void CreateStoreItemCommand_WhenCodeAlreadyExist(HttpClient client, CreateStoreItem.Request command,
        HttpResponseMessage response)
    {
        "Given CreateStoreItem command"
            .x(() => client = _factory.CreateClient());

        "With valid data"
            .x(() => command = new CreateStoreItem.Request("Code001", "Data001"));

        "When I call the API"
            .x(async () =>
            {
                const string requestUri = "/api/v2/store-items/create-store-item";

                response = await client.PostAsJsonAsync(requestUri, command);
            });

        "Then the status code should indicate UnprocessableEntity"
            .x(() => response.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity));

        "And a CommandResult with created status"
            .x(async () =>
            {
                var result = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
                result.Should()
                    .NotBeNull()
                    .And.BeOfType<HttpValidationProblemDetails>()
                    .Which.Errors.Should()
                    .ContainEquivalentOf(new KeyValuePair<string, string[]>("Code",
                        new[] {"Code 'Code001' already exists."}));
            });
    }

    [Scenario]
    public void ModifyStoreItemCommand(HttpClient client, Guid data, Guid createResult,
        ModifyStoreItem.Request modifyCommand, CommandResult modifyResult,
        HttpResponseMessage modifyResponse)
    {
        "Given ModifyStoreItem command"
            .x(() => client = _factory.CreateClient());

        "For newly registered test user"
            .x(() => client.DefaultRequestHeaders.Add(TestAuthHandler.TEST_USER_ID_HEADER,
                Guid.NewGuid().ToString()));

        "On existing store item"
            .x(async () =>
            {
                const string createRequestUri = "/api/v2/store-items/create-store-item";

                var code = Guid.NewGuid();
                data = Guid.NewGuid();

                var createCommand = new CreateStoreItem.Request(code.ToString(), data.ToString());
                var createResponse = await client.PostAsJsonAsync(createRequestUri, createCommand);

                var result =
                    await createResponse.Content.ReadFromJsonAsync<JsonElement>(_jsonSerializerOptions);

                createResult = result.GetProperty("id").GetGuid();
            });

        "And modifying the data property"
            .x(() => modifyCommand =
                new ModifyStoreItem.Request(createResult, $"{data.ToString()}-modified"));

        "When I call the API"
            .x(async () =>
            {
                const string modifyRequestUri = "/api/v2/store-items/modify-store-item";

                modifyResponse = await client.PostAsJsonAsync(modifyRequestUri, modifyCommand);
            });

        "Then the status code should indicate Success"
            .x(() => modifyResponse.Should()
                .NotBeNull()
                .And.BeOfType<HttpResponseMessage>()
                .Which.StatusCode.Should().Be(HttpStatusCode.NoContent));
    }
}
