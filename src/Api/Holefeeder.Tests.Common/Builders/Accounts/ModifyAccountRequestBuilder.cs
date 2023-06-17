using DrifterApps.Seeds.Testing;
using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class ModifyAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = new Faker<Request>();

    public ModifyAccountRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyAccountRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static ModifyAccountRequestBuilder GivenAModifyAccountRequest() => new();

    public static ModifyAccountRequestBuilder GivenAnInvalidModifyAccountRequest() =>
        new ModifyAccountRequestBuilder().WithNoId();
}
