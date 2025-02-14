using Bogus;

using DrifterApps.Seeds.Testing;

using Holefeeder.Ui.Shared.Models;

namespace Holefeeder.UnitTests.Ui.Common.Builders;

internal sealed class AccountBuilder : FakerBuilder<Account>
{
    protected override Faker<Account> Faker { get; } = CreateFaker<Account>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Name, f => f.Finance.AccountName())
        .RuleFor(x => x.Type, f => f.PickRandom<AccountType>())
        .RuleFor(x => x.OpenBalance, f => f.Finance.Amount(min: 0))
        .RuleFor(x => x.OpenDate, f => f.Date.PastDateOnly())
        .RuleFor(x => x.TransactionCount, f => f.Random.Int(min: 0))
        .RuleFor(x => x.Balance, f => f.Finance.Amount(min: 0))
        .RuleFor(x => x.Updated, f => f.Date.PastDateOnly())
        .RuleFor(x => x.Description, f => f.Lorem.Sentence())
        .RuleFor(x => x.Favorite, f => f.Random.Bool())
        .RuleFor(x => x.Inactive, f => f.Random.Bool());
}
