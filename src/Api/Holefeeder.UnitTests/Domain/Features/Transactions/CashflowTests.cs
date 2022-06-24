using System;

using AutoBogus;

using FluentAssertions;
using FluentAssertions.Execution;

using Holefeeder.Domain.Features.Transactions;

using Xunit;

namespace Holefeeder.UnitTests.Domain.Features.Transactions;

public class CashflowTests
{
    private readonly AutoFaker<Cashflow> _faker = new();

    [Fact]
    public void GivenConstructor_WhenIdEmpty_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.Id, Guid.Empty);

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Id is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenEffectiveDateIsMissing_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.EffectiveDate, default(DateTime));

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("EffectiveDate is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenAmountIsNegative_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.Amount,
            faker => faker.Finance.Amount(Decimal.MinValue, Decimal.MinusOne));

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Amount cannot be negative")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenFrequencyIsNotPositive_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.Frequency,
            faker => faker.Random.Int(int.MinValue, 0));

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Frequency must be positive")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenRecurrenceIsNegative_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.Recurrence,
            faker => faker.Random.Int(int.MinValue, 0));

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("Recurrence cannot be negative")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenAccountIdEmpty_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(transaction => transaction.AccountId, Guid.Empty);

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("AccountId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenCategoryIdEmpty_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(transaction => transaction.CategoryId, Guid.Empty);

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("CategoryId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenConstructor_WhenUserIdEmpty_ThenThrowException()
    {
        // arrange
        _faker.RuleFor(cashflow => cashflow.UserId, Guid.Empty);

        // act
        Action action = () => _ = _faker.Generate();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage("UserId is required")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenCancel_WhenCashflowInactive_ThenThrowException()
    {
        // arrange
        var cashflow = _faker.RuleFor(cashflow => cashflow.Inactive, true).Generate();

        // act
        Action action = () => _ = cashflow.Cancel();

        // assert
        action.Should().Throw<TransactionDomainException>()
            .WithMessage($"Cashflow {cashflow.Id} already inactive")
            .And
            .Context.Should().Be(nameof(Cashflow));
    }

    [Fact]
    public void GivenCancel_WhenCashflowActive_ThenCashflowIsInactive()
    {
        // arrange
        var cashflow = _faker.RuleFor(cashflow => cashflow.Inactive, false).Generate();

        // act
        var result = cashflow.Cancel();

        // assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.Inactive.Should().BeTrue();
    }

    [Fact]
    public void GivenSetTags_WhenEmptyList_ThenTagListIsEmpty()
    {
        // arrange
        var cashflow = _faker.Generate();

        // act
        var result = cashflow.SetTags(Array.Empty<string>());

        // assert
        result.Tags.Should().BeEmpty();
    }

    [Fact]
    public void GivenSetTags_WhenAddingTags_ThenTagListIsSet()
    {
        // arrange
        var cashflow = _faker.Generate();
        var newTags = AutoFaker.Generate<string[]>();

        // act
        var result = cashflow.SetTags(newTags);

        // assert
        result.Tags.Should().Contain(newTags);
    }
}
