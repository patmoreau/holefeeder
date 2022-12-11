using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using static Holefeeder.Application.Features.Transactions.Commands.MakePurchase;

namespace Holefeeder.UnitTests.Application.Features.Transactions.Commands;

public class MakePurchaseTests
{
    private readonly ICashflowRepository _cashflowRepositoryMock = Substitute.For<ICashflowRepository>();
    private readonly AutoFaker<Request> _faker = new();
    private readonly ITransactionRepository _repositoryMock = Substitute.For<ITransactionRepository>();

    private readonly IUserContext _userContextMock = MockHelper.CreateUserContext();

    public MakePurchaseTests()
    {
        _faker.RuleFor(x => x.Cashflow, _ => null);

        _repositoryMock.AccountExists(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _repositoryMock.CategoryExists(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
    }

    [Fact]
    public async Task GivenValidator_WhenAccountIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.AccountId, Guid.Empty).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.AccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenAccountIdDoesNotExists_ThenError()
    {
        // arrange
        var request = _faker.Generate();
        _repositoryMock.AccountExists(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.AccountId);
    }

    [Fact]
    public async Task GivenValidator_WhenCategoryIdIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.CategoryId, Guid.Empty).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CategoryId);
    }

    [Fact]
    public async Task GivenValidator_WhenCategoryIdDoesNotExists_ThenError()
    {
        // arrange
        var request = _faker.Generate();
        _repositoryMock.CategoryExists(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.CategoryId);
    }

    [Fact]
    public async Task GivenValidator_WhenDateIsEmpty_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Date, _ => default).Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Date);
    }

    [Fact]
    public async Task GivenValidator_WhenAmountNotGreaterThanZero_ThenError()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Amount, faker => faker.Random.Decimal(Decimal.MinValue, Decimal.Zero))
            .Generate();

        var validator = new Validator(_userContextMock, _repositoryMock);

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

        var validator = new Validator(_userContextMock, _repositoryMock);

        // act
        var result = await validator.TestValidateAsync(request);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestValid_ThenReturnId()
    {
        // arrange
        var request = _faker.RuleFor(x => x.Cashflow, _ => null).Generate();

        var handler = new Handler(_userContextMock, _repositoryMock, _cashflowRepositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenHandler_WhenRequestWithCashflowValid_ThenReturnId()
    {
        // arrange
        var request = _faker.Generate();

        var handler = new Handler(_userContextMock, _repositoryMock, _cashflowRepositoryMock);

        // act
        var result = await handler.Handle(request, default);

        // assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GivenHandler_WhenTransactionDomainException_ThenRollbackTransaction()
    {
        // arrange
        var request = _faker.Generate();

        _repositoryMock.SaveAsync(Arg.Any<Transaction>(), Arg.Any<CancellationToken>())
            .Throws(new TransactionDomainException(
                nameof(GivenHandler_WhenTransactionDomainException_ThenRollbackTransaction), nameof(Transaction)));

        var handler = new Handler(_userContextMock, _repositoryMock, _cashflowRepositoryMock);

        // act
        Func<Task> action = () => handler.Handle(request, default);

        // assert
        using var scope = new AssertionScope();
        await action.Should().ThrowAsync<TransactionDomainException>();
        _repositoryMock.UnitOfWork.Received(1).Dispose();
    }
}
