using System;
using System.Threading;
using System.Threading.Tasks;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using FluentValidation.TestHelper;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Factories;

using MediatR;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using Xunit;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class ModifyCashflowTests
{
    private readonly AutoFaker<Request> _faker = new();
    private readonly CashflowFactory _factory = new();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();
    private readonly ICashflowRepository _cashflowRepositoryMock = Substitute.For<ICashflowRepository>();

    public ModifyCashflowTests()
    {
        _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.One, decimal.MaxValue));
    }

    [Fact]
    public async Task GivenValidator_WhenIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Id, Guid.Empty).Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Id);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountIsNotGreaterThanZero_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Amount, faker => faker.Finance.Amount(decimal.MinValue, decimal.Zero))
            .Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public async Task GivenValidator_WhenRequestValid_ThenNoErrors()
    {
        // arrange
        var request = _faker.Generate();

        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestValid_ThenSuccess()
    {
        // arrange
        var request = _faker.Generate();

        _cashflowRepositoryMock.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(_factory.Generate());

        var handler = new Handler(_userContextMock, _cashflowRepositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task GivenHandler_WhenIdNotFound_ThenThrowException()
    {
        // arrange
        var request = _faker.Generate();

        _cashflowRepositoryMock.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Cashflow?) null);

        var handler = new Handler(_userContextMock, _cashflowRepositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        await action.Should().ThrowAsync<CashflowNotFoundException>();
    }

    [Fact]
    public async Task GivenHandler_WhenTransactionDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = _faker.Generate();

        _cashflowRepositoryMock.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(_factory.Generate());

        _cashflowRepositoryMock.SaveAsync(Arg.Any<Cashflow>(), Arg.Any<CancellationToken>())
            .Throws(new TransactionDomainException(
                nameof(GivenHandler_WhenTransactionDomainException_ThenRollbackTransaction), nameof(Transaction)));

        var handler = new Handler(_userContextMock, _cashflowRepositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        await action.Should().ThrowAsync<TransactionDomainException>();
        _cashflowRepositoryMock.UnitOfWork.Received(1).Dispose();
    }
}
