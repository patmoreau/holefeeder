using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.SeedWork;

namespace Holefeeder.Tests.Common.Builders.MyData;

internal sealed class MyDataAccountDtoBuilder : RootBuilder<MyDataAccountDto>
{
    protected override Faker<MyDataAccountDto> Faker { get; } = new AutoFaker<MyDataAccountDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(AccountType.List.ToArray()));

    public static MyDataAccountDtoBuilder GivenMyAccountData() => new();
}
