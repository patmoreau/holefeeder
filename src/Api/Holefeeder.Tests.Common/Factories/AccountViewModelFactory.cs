using AutoBogus;

using Bogus.Extensions;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class AccountViewModelFactory : AutoFaker<AccountViewModel>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    public AccountViewModelFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Type, faker => faker.PickRandom(Enumeration.GetAll<AccountType>()));
        RuleFor(x => x.Name, faker => faker.Random.Words().ClampLength(min: 1, max:100));
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.TransactionCount, faker => faker.Random.Int(min: 0, max: 100));
        RuleFor(x => x.Balance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.Updated, faker => faker.Date.Recent().Date);
        RuleFor(x => x.Inactive, false);
    }
}
