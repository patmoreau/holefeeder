using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.SeedWork;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.Accounts.Queries.GetAccount;

namespace Holefeeder.UnitTests.Application.Features.Accounts.Queries;

public class GetAccountTests
{
    private readonly AutoFaker<Request> _faker = new();

    private readonly AccountViewModel _dummy = new AutoFaker<AccountViewModel>().Generate();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly IAccountQueriesRepository _repositoryMock = Substitute.For<IAccountQueriesRepository>();

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenHandler_WhenIdNotFound_ThenThrowException()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<AccountNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_dummy);

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(_dummy);
    }
}
