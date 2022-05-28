using AutoBogus;

using Bogus.Extensions;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class AccountFactory : AutoFaker<Account>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    public AccountFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Type, faker => faker.PickRandom(Enumeration.GetAll<AccountType>()));
        RuleFor(x => x.Name, faker => faker.Random.String2(minLength: 1, maxLength: 100));
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.Inactive, false);
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
        RuleFor(x => x.Cashflows, Array.Empty<Guid>());
    }
}
