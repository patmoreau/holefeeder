using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;

using FluentValidation;
using FluentValidation.TestHelper;

using Holefeeder.Application.Features.StoreItems.Exceptions;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.Application.SeedWork;

using NSubstitute;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Queries.GetStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Queries;

public class GetStoreItemTests
{
    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = new Request(Guid.Empty);
        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id)
            .WithErrorMessage("'Id' must not be empty.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public async Task GivenHandler_WhenIdNotFound_ThenThrowException()
    {
        // arrange
        var request = new AutoFaker<Request>();
        var userContextMock = Substitute.For<IUserContext>();
        var repositoryMock = Substitute.For<IStoreItemsQueriesRepository>();
        var handler = new Handler(userContextMock, repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<StoreItemNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var userId = AutoFaker.Generate<Guid>();
        var request = new AutoFaker<Request>().Generate();
        var expected = new AutoFaker<StoreItemViewModel>().Generate();
        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.UserId.Returns(userId);
        var repositoryMock = Substitute.For<IStoreItemsQueriesRepository>();
        repositoryMock.FindByIdAsync(Arg.Is(userId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(expected);
        var handler = new Handler(userContextMock, repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(expected);
    }
}
