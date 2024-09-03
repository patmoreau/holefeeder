using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Accounts;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;

namespace Holefeeder.Tests.Common.Builders.Accounts;

internal class OpenAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.Type, faker => faker.PickRandom<AccountType>(AccountType.List))
        .RuleFor(x => x.Name, faker => faker.Lorem.Word())
        .RuleFor(x => x.OpenBalance, MoneyBuilder.Create().Build())
        .RuleFor(x => x.Description, faker => faker.Lorem.Sentence())
        .RuleFor(x => x.OpenDate, faker => faker.Date.PastDateOnly());

    public OpenAccountRequestBuilder WithName(string name)
    {
        Faker.RuleFor(x => x.Name, name);
        return this;
    }

    public OpenAccountRequestBuilder WithNoName()
    {
        Faker.RuleFor(x => x.Name, string.Empty);
        return this;
    }

    public OpenAccountRequestBuilder WithNoType()
    {
        Faker.RuleFor(x => x.Type, faker => null!);
        return this;
    }

    public OpenAccountRequestBuilder WithNoOpenDate()
    {
        Faker.RuleFor(x => x.OpenDate, DateOnly.MinValue);
        return this;
    }

    public static OpenAccountRequestBuilder GivenAnOpenAccountRequest() => new();

    public static OpenAccountRequestBuilder GivenAnInvalidOpenAccountRequest() =>
        new OpenAccountRequestBuilder().WithNoName();
}
