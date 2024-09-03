using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.Domain.ValueObjects;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyTransactionRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateFaker()
        .RuleFor(x => x.Id, faker => (TransactionId)faker.RandomGuid())
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => (AccountId)faker.RandomGuid())
        .RuleFor(x => x.CategoryId, faker => (CategoryId)faker.RandomGuid())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public ModifyTransactionRequestBuilder OfAmount(Money amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyTransactionRequestBuilder WithId(TransactionId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, TransactionId.Empty);
        return this;
    }

    public ModifyTransactionRequestBuilder WithAccount(Account account)
    {
        Faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoAccount()
    {
        Faker.RuleFor(x => x.AccountId, faker => faker.PickRandom(AccountId.Empty, null));
        return this;
    }

    public ModifyTransactionRequestBuilder WithCategory(Category category)
    {
        Faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoCategory()
    {
        Faker.RuleFor(x => x.CategoryId, faker => faker.PickRandom(CategoryId.Empty, null));
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoDate()
    {
        Faker.RuleFor(x => x.Date, DateOnly.MinValue);
        return this;
    }

    public static ModifyTransactionRequestBuilder GivenAModifyTransactionRequest() => new();

    public static ModifyTransactionRequestBuilder GivenAnInvalidModifyTransactionRequest() =>
        new ModifyTransactionRequestBuilder().WithNoId();
}
