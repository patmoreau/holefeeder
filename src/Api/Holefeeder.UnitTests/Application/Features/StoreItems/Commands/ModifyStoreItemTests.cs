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

using MediatR;

using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.UnitTests.Application.Features.StoreItems.Commands;

public class ModifyStoreItemTests
{
    private readonly Guid _id = AutoFaker.Generate<Guid>();
    private readonly Guid _userId = AutoFaker.Generate<Guid>();
    private readonly string _data = AutoFaker.Generate<string>();
    private readonly StoreItem _storeItem = AutoFaker.Generate<StoreItem>();
    private readonly IUserContext _userContextMock = Substitute.For<IUserContext>();
    private readonly IStoreItemsRepository _repositoryMock = Substitute.For<IStoreItemsRepository>();
    private readonly ILogger<Handler> _loggerMock = Substitute.For<ILogger<Handler>>();

    public ModifyStoreItemTests()
    {
        _userContextMock.UserId.Returns(_userId);
    }

    [Fact]
    public void GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = new Request(Guid.Empty, _data);

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
    public void GivenValidator_WhenDataIsEmpty_ThenError()
    {
        // arrange
        var request = new Request(_id, string.Empty);
        var validator = new Validator();

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
        var request = new Request(_id, _data);
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
        var request = new AutoFaker<Request>().Generate();
        var handler = new Handler(_userContextMock, _repositoryMock, _loggerMock);
        _repositoryMock.FindByIdAsync(Arg.Is(_userId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_storeItem);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task GivenHandler_WhenStoreItemNotFound_ThenThrowException()
    {
        // arrange
        var request = new AutoFaker<Request>().Generate();
        var handler = new Handler(_userContextMock, _repositoryMock, _loggerMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<StoreItemNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenObjectStoreDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = new AutoFaker<Request>().Generate();
        _repositoryMock.FindByIdAsync(Arg.Is(_userId), Arg.Is(request.Id), Arg.Any<CancellationToken>())
            .Returns(_storeItem);
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
