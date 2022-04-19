using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using FluentValidation.TestHelper;

using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.StoreItem;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class CreateStoreItemTests
{
    private readonly AutoFaker<Request> _faker = new();

    private readonly StoreItem _storeItemDummy = AutoFaker.Generate<StoreItem>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly ILogger<Handler> _loggerMock = MockHelper.CreateLogger<Handler>();
    private readonly IStoreItemsRepository _repositoryMock = Substitute.For<IStoreItemsRepository>();

    [Fact]
    public void GivenValidator_WhenCodeIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Code, string.Empty).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code);
    }

    [Fact]
    public void GivenValidator_WhenCodeAlreadyExists_ThenError()
    {
        // arrange
        var code = AutoFaker.Generate<string>();
        var request = _faker.RuleFor(x => x.Code, _ => code).Generate();
        _repositoryMock
            .FindByCodeAsync(Arg.Is(_userContextMock.UserId), Arg.Is(code), Arg.Any<CancellationToken>())
            .Returns(_storeItemDummy);

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code);
    }

    [Fact]
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Data, string.Empty).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

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

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestValid_ThenReturnId()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock, _loggerMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.SaveAsync(Arg.Any<StoreItem>(), Arg.Any<CancellationToken>())
            .Throws(new ObjectStoreDomainException(
                nameof(GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction)));

        var handler = new Handler(_userContextMock, _repositoryMock, _loggerMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        await action.Should().ThrowAsync<ObjectStoreDomainException>();
        _repositoryMock.UnitOfWork.Received(1).Dispose();
    }
}
