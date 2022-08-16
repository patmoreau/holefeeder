using AutoBogus;

using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class AccountFactory : AutoFaker<Account>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    public AccountFactory()
    {
        CustomInstantiator(faker => new Account(
            faker.Random.Guid(),
            faker.PickRandom(AccountType.List.ToArray()),
            faker.Random.String2(minLength: 1, maxLength: 100),
            faker.Date.Past().Date,
            faker.Random.Guid()));
        Ignore(x => x.Type);
        Ignore(x => x.Name);
        Ignore(x => x.OpenDate);
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.Inactive, false);
        RuleFor(x => x.Cashflows, Array.Empty<Guid>());
    }
}
