using DrifterApps.Seeds.Testing;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataAccountDtoBuilder : FakerBuilder<MyDataAccountDto>
{
    protected override Faker<MyDataAccountDto> Faker { get; } = new Faker<MyDataAccountDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(AccountType.List.ToArray()));

    public static MyDataAccountDtoBuilder GivenMyAccountData() => new();
}
