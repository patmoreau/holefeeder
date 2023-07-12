using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyTransactionRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Date, faker => faker.Date.RecentDateOnly())
        .RuleFor(x => x.Amount, faker => faker.Finance.Amount())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.AccountId, faker => faker.Random.Guid())
        .RuleFor(x => x.CategoryId, faker => faker.Random.Guid())
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public ModifyTransactionRequestBuilder OfAmount(decimal amount)
    {
        Faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyTransactionRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public ModifyTransactionRequestBuilder WithAccount(Account account)
    {
        Faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithCategory(Category category)
    {
        Faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public static ModifyTransactionRequestBuilder GivenAModifyTransactionRequest() => new();

    public static ModifyTransactionRequestBuilder GivenAnInvalidModifyTransactionRequest() =>
        new ModifyTransactionRequestBuilder().WithNoId();
}
