using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Users.Core;
using Holefeeder.Service.Users.Core.GetCurrentUser;
using Holefeeder.ServiceTests.Users.TestKit;

namespace Holefeeder.ServiceTests.Users.Core.GetCurrentUser;

public class GetCurrentUserUseCaseTests
{
    private readonly RegisteredUsersInMemory _users = new();
    private readonly GetCurrentUserUseCase _useCase;

    public GetCurrentUserUseCaseTests() => _useCase = new GetCurrentUserUseCase(_users);

    [Fact]
    public async Task HandleAsync_ReturnsTheUserId_WhenTheCallerIsRegistered()
    {
        var identityObjectId = AnIdentityObjectId();
        var id = AnId();
        _users.Add(identityObjectId, id);
        _users.Add(AnIdentityObjectId(), AnId());

        var result = await _useCase.HandleAsync(new GetCurrentUserRequest(identityObjectId), CancellationToken.None);

        result.Should().BeSuccessful().And.WithValue(id);
    }

    [Fact]
    public async Task HandleAsync_FailsWithNotFound_WhenTheCallerIsNotRegistered()
    {
        _users.Add(AnIdentityObjectId(), AnId());

        var result = await _useCase.HandleAsync(new GetCurrentUserRequest(AnIdentityObjectId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(RegisteredUserErrors.NotFound);
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenUsersCannotBeRead()
    {
        var failure = new ResultError("users-unavailable", "Users are unavailable.");
        _users.FailWith(failure);

        var result = await _useCase.HandleAsync(new GetCurrentUserRequest(AnIdentityObjectId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
    }
}
