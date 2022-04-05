using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;

using FluentValidation;
using FluentValidation.TestHelper;

using Holefeeder.Application.Features.StoreItems.Exceptions;
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
    private readonly Guid _userId = AutoFaker.Generate<Guid>();
    private readonly string _code = AutoFaker.Generate<string>();
    private readonly string _data = AutoFaker.Generate<string>();
    private readonly IUserContext _userContextMock = Substitute.For<IUserContext>();
    private readonly IStoreItemsRepository _repositoryMock = Substitute.For<IStoreItemsRepository>();
    private readonly ILogger<Handler> _loggerMock = Substitute.For<ILogger<Handler>>();

    public CreateStoreItemTests()
    {
        _userContextMock.UserId.Returns(_userId);
    }

    [Fact]
    public void GivenValidator_WhenCodeIsEmpty_ThenError()
    {
        // arrange
        var request = new Request(string.Empty, _data);
        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code)
            .WithErrorMessage("'Code' must not be empty.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public void GivenValidator_WhenCodeAlreadyExists_ThenError()
    {
        // arrange
        _repositoryMock.FindByCodeAsync(Arg.Any<Guid>(), Arg.Is(_code), Arg.Any<CancellationToken>())
            .Returns(AutoFaker.Generate<StoreItem>());
        var request = new Request(_code, _data);
        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Code)
            .WithErrorMessage($"Code '{_code}' already exists.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("AlreadyExistsValidator");
    }

    [Fact]
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = new Request(_code, string.Empty);
        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = validator.TestValidate(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Data)
            .WithErrorMessage("'Data' must not be empty.")
            .WithSeverity(Severity.Error)
            .WithErrorCode("NotEmptyValidator");
    }

    [Fact]
    public void GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = new Request(_code, _data);
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
        var request = new AutoFaker<Request>();
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
        var request = new AutoFaker<Request>();
        _repositoryMock.SaveAsync(Arg.Any<StoreItem>(), Arg.Any<CancellationToken>()).Throws(
            new ObjectStoreDomainException(
                nameof(GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction)));
        var handler = new Handler(_userContextMock, _repositoryMock, _loggerMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<ObjectStoreDomainException>();
        _repositoryMock.UnitOfWork.Received(1).Dispose();
    }
}
