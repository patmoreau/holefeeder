using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyTransactionRequestBuilder : RootBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new AutoFaker<Request>()
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
