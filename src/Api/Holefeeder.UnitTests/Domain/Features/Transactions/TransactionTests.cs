using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Tests.Common.Builders.Transactions;

namespace Holefeeder.UnitTests.Domain.Features.Transactions;

public class TransactionTests
{
    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        var builder = TransactionBuilder.GivenATransaction().WithNoId();

        // act
        Action action = () => _ = builder.Build();

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
        var builder = TransactionBuilder.GivenATransaction().WithNoDate();

        // act
        Action action = () => _ = builder.Build();

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
        var builder = TransactionBuilder.GivenATransaction().WithNegativeAmount();

        // act
        Action action = () => _ = builder.Build();

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
        var builder = TransactionBuilder.GivenATransaction().WithNoAccount();

        // act
        Action action = () => _ = builder.Build();

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
        var builder = TransactionBuilder.GivenATransaction().WithNoCategory();

        // act
        Action action = () => _ = builder.Build();

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
        var builder = TransactionBuilder.GivenATransaction().WithNoUser();

        // act
        Action action = () => _ = builder.Build();

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
        var transaction = TransactionBuilder.GivenATransaction().Build();

        // act
        var result = transaction.SetTags(Array.Empty<string>());

        // assert
        result.Tags.Should().BeEmpty();
    }

    [Fact]
    public void GivenSetTags_WhenAddingTags_ThenTagListIsSet()
    {
        // arrange
        var transaction = TransactionBuilder.GivenATransaction().Build();
        string[]? newTags = AutoFaker.Generate<string[]>();

        // act
        var result = transaction.SetTags(newTags);

        // assert
        result.Tags.Should().Contain(newTags);
    }

    [Fact]
    public void GivenApplyCashflow_WhenCashflowIdEmpty_ThenThrowException()
    {
        // arrange
        var transaction = TransactionBuilder.GivenATransaction().Build();

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
        var transaction = TransactionBuilder.GivenATransaction().Build();

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
        var transaction = TransactionBuilder.GivenATransaction().Build();
        var cashflowId = AutoFaker.Generate<Guid>();
        var cashflowDate = AutoFaker.Generate<DateTime>();

        // act
        var result = transaction.ApplyCashflow(cashflowId, cashflowDate);

        // assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.CashflowId.Should().Be(cashflowId);
            result.CashflowDate.Should().Be(cashflowDate);
        }
    }
}
