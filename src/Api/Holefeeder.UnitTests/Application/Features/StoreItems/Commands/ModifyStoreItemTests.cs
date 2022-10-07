using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.StoreItems.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using MediatR;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class ModifyStoreItemTests
{
    private readonly AutoFaker<Request> _faker = new();
    private readonly IStoreItemsRepository _repositoryMock = Substitute.For<IStoreItemsRepository>();

    private readonly StoreItem _storeItemDummy = AutoFaker.Generate<StoreItem>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();

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
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Data, string.Empty).Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data);
    }

    [Fact]
    public void GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestValid_ThenReturnUnitValue()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock);
        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_storeItemDummy);

        // act
        var result = await handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        result.Should().Be(Unit.Value);
        await _repositoryMock.Received(1)
            .SaveAsync(Arg.Is<StoreItem>(x => x.Id == _storeItemDummy.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenHandler_WhenStoreItemNotFound_ThenThrowException()
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
    public async Task GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.FindByIdAsync(Arg.Is(_userContextMock.UserId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_storeItemDummy);
        _repositoryMock.SaveAsync(Arg.Is<StoreItem>(x => x.Id == _storeItemDummy.Id), Arg.Any<CancellationToken>())
            .Throws(new ObjectStoreDomainException(
                nameof(GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction)));

        var handler = new Handler(_userContextMock, _repositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        await action.Should().ThrowAsync<ObjectStoreDomainException>();
        _repositoryMock.UnitOfWork.Received(1).Dispose();
    }
}
