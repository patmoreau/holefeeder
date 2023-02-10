using static Holefeeder.Application.Features.Accounts.Commands.ModifyAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class ModifyAccountRequestBuilder : IBuilder<Request>
{
    private readonly Faker<Request> _faker = new AutoFaker<Request>();

    public Request Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public ModifyAccountRequestBuilder WithId(Guid id)
    {
        _faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyAccountRequestBuilder WithNoId()
    {
        _faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public static ModifyAccountRequestBuilder GivenAModifyAccountRequest() => new();

    public static ModifyAccountRequestBuilder GivenAnInvalidModifyAccountRequest() =>
        new ModifyAccountRequestBuilder().WithNoId();
}
