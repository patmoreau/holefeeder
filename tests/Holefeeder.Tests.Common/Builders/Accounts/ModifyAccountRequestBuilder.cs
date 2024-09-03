using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class ModifyAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
            .RuleFor(x => x.Id, faker => (AccountId)faker.RandomGuid())
            .RuleFor(x => x.Name, faker => faker.Lorem.Word())
            .RuleFor(x => x.OpenBalance, faker => MoneyBuilder.Create().Build())
            .RuleFor(x => x.Description, faker => faker.Lorem.Sentence());

    public ModifyAccountRequestBuilder WithId(AccountId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyAccountRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, AccountId.Empty);
        return this;
    }

    public static ModifyAccountRequestBuilder GivenAModifyAccountRequest() => new();

    public static ModifyAccountRequestBuilder GivenAnInvalidModifyAccountRequest() =>
        new ModifyAccountRequestBuilder().WithNoId();
}
