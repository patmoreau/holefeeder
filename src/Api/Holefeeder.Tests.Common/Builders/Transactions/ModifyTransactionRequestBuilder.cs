using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyTransaction;

namespace Holefeeder.Tests.Common.Builders.Transactions;

internal class ModifyTransactionRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>()
        .RuleFor(x => x.Tags, faker => faker.Lorem.Words(faker.Random.Int(1, 10)).Distinct().ToArray());

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public ModifyTransactionRequestBuilder OfAmount(decimal amount)
    {
        _faker.RuleFor(x => x.Amount, amount);
        return this;
    }

    public ModifyTransactionRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public ModifyTransactionRequestBuilder WithAccount(Account account)
    {
        _faker.RuleFor(x => x.AccountId, account.Id);
        return this;
    }

    public ModifyTransactionRequestBuilder WithCategory(Category category)
    {
        _faker.RuleFor(x => x.CategoryId, category.Id);
        return this;
    }

    public static ModifyTransactionRequestBuilder GivenAModifyTransactionRequest() => new();

    public static ModifyTransactionRequestBuilder GivenAnInvalidModifyTransactionRequest() =>
        new ModifyTransactionRequestBuilder().WithNoId();
}
