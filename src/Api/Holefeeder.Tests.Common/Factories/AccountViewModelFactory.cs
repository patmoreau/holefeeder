using AutoBogus;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class AccountViewModelFactory : AutoFaker<AccountViewModel>
{
    private const decimal OPEN_BALANCE_MAX = 10000m;

    public AccountViewModelFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Type, faker => faker.PickRandom(AccountType.List.ToArray()));
        RuleFor(x => x.Name, faker => faker.Random.String2(1, 100));
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.OpenBalance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.OpenDate, faker => faker.Date.Past().Date);
        RuleFor(x => x.Description, faker => faker.Random.Words());
        RuleFor(x => x.TransactionCount, faker => faker.Random.Int(0, 100));
        RuleFor(x => x.Balance, faker => faker.Finance.Amount(max: OPEN_BALANCE_MAX));
        RuleFor(x => x.Updated, faker => faker.Date.Recent().Date);
        RuleFor(x => x.Inactive, false);
    }
}
