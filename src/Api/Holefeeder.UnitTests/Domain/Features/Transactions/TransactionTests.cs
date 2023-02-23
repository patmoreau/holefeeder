using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Factories;

namespace Holefeeder.UnitTests.Domain.Features.Transactions;

public class TransactionTests
{
    private readonly TransactionFactory _factory = new();

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.Id, Guid.Empty);

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenConstructor_WhenDateIsMissing_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.Date, default(DateTime));

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Date is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenConstructor_WhenAmountIsNegative_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.Amount,
            faker => faker.Finance.Amount(decimal.MinValue, decimal.MinusOne));

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Amount cannot be negative")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenConstructor_WhenAccountIdEmpty_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.AccountId, Guid.Empty);

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("AccountId is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenConstructor_WhenCategoryIdEmpty_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.CategoryId, Guid.Empty);

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("CategoryId is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange
        _factory.RuleFor(transaction => transaction.UserId, Guid.Empty);

        // act
        Action action = () => _ = _factory.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenSetTags_WhenEmptyList_ThenTagListIsEmpty()
    {
        // arrange
        Transaction? transaction = _factory.Generate();

        // act
        Transaction result = transaction.SetTags(Array.Empty<string>());

        // assert
        result.Tags.Should().BeEmpty();
    }

    [Fact]
    public void GivenSetTags_WhenAddingTags_ThenTagListIsSet()
    {
        // arrange
        Transaction? transaction = _factory.Generate();
        string[]? newTags = AutoFaker.Generate<string[]>();

        // act
        Transaction result = transaction.SetTags(newTags);

        // assert
        result.Tags.Should().Contain(newTags);
    }

    [Fact]
    public void GivenApplyCashflow_WhenCashflowIdEmpty_ThenThrowException()
    {
        // arrange
        Transaction? transaction = _factory.Generate();

        // act
        Action action = () => _ = transaction.ApplyCashflow(Guid.Empty, AutoFaker.Generate<DateTime>());

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("CashflowId is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenApplyCashflow_WhenDateIsMissing_ThenThrowException()
    {
        // arrange
        Transaction? transaction = _factory.Generate();

        // act
        Action action = () => _ = transaction.ApplyCashflow(AutoFaker.Generate<Guid>(), default);

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("CashflowDate is required")
            .And
            .Context.Should().Be(nameof(Transaction));
    }

    [Fact]
    public void GivenApplyCashflow_WhenValid_ThenSetTransaction()
    {
        // arrange
        Transaction? transaction = _factory.Generate();
        Guid cashflowId = AutoFaker.Generate<Guid>();
        DateTime cashflowDate = AutoFaker.Generate<DateTime>();

        // act
        Transaction result = transaction.ApplyCashflow(cashflowId, cashflowDate);

        // assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.CashflowId.Should().Be(cashflowId);
            result.CashflowDate.Should().Be(cashflowDate);
        }
    }
}
