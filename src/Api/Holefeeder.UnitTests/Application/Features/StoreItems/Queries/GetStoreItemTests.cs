using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using Bogus;

using FluentAssertions;

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
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    private readonly StoreItemViewModel _storeItemDummy = AutoFaker.Generate<StoreItemViewModel>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly IStoreItemsQueriesRepository _repositoryMock = Substitute.For<IStoreItemsQueriesRepository>();

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
        await action.Should().ThrowAsync<StoreItemNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenIdFound_ThenReturnResult()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_storeItemDummy);

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(_storeItemDummy);
    }
}
