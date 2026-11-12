using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Users.Core;
using Holefeeder.Service.Users.Core.RegisterUser;
using Holefeeder.ServiceTests.Users.TestKit;

namespace Holefeeder.ServiceTests.Users.Core.RegisterUser;

public class RegisterUserUseCaseTests
{
    private readonly RegisteredUsersInMemory _users = new();
    private readonly RegisterUserUseCase _useCase;

    public RegisterUserUseCaseTests() => _useCase = new RegisterUserUseCase(_users);

    [Fact]
    public async Task HandleAsync_SavesTheCallerWithTheirIdentity_WhenNotRegisteredYet()
    {
        var identityObjectId = AnIdentityObjectId();

        var result = await _useCase.HandleAsync(new RegisterUserRequest(identityObjectId), CancellationToken.None);

        result.Should().BeSuccessful();
        var saved = _users.Saved.Should().ContainSingle().Subject;
        saved.Id.Should().Be(result.Value);
        saved.Id.Should().NotBe(Id.Empty);
        saved.IdentityObjectId.Should().Be(identityObjectId);
    }

    [Fact]
    public async Task HandleAsync_SeedsTheTransferCategories_WhenNotRegisteredYet()
    {
        await _useCase.HandleAsync(new RegisterUserRequest(AnIdentityObjectId()), CancellationToken.None);

        var categories = _users.Saved.Should().ContainSingle().Subject.Categories;
        categories.Should().BeEquivalentTo(
            [
                new { Type = CategoryType.Expense, Name = SystemCategory.TransferOutName },
                new { Type = CategoryType.Gain, Name = SystemCategory.TransferInName }
            ]);
        categories.Select(category => category.Id).Should().OnlyHaveUniqueItems().And.NotContain(Id.Empty);
    }

    [Fact]
    public async Task HandleAsync_FailsWithAlreadyRegistered_WhenTheCallerIsRegistered()
    {
        var identityObjectId = AnIdentityObjectId();
        _users.Add(identityObjectId, AnId());

        var result = await _useCase.HandleAsync(new RegisterUserRequest(identityObjectId), CancellationToken.None);

        result.Should().BeFailure().And.WithError(RegisteredUserErrors.AlreadyRegistered);
        _users.Saved.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ReturnsTheFailure_WhenUsersCannotBeRead()
    {
        var failure = new ResultError("users-unavailable", "Users are unavailable.");
        _users.FailWith(failure);

        var result = await _useCase.HandleAsync(new RegisterUserRequest(AnIdentityObjectId()), CancellationToken.None);

        result.Should().BeFailure().And.WithError(failure);
        _users.Saved.Should().BeEmpty();
    }
}
