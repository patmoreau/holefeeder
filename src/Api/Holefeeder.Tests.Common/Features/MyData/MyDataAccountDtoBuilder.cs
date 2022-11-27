using AutoBogus;

using Bogus;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Tests.Common.Builders;

namespace Holefeeder.Tests.Common.Features.MyData;

internal sealed class MyDataAccountDtoBuilder : IBuilder<MyDataAccountDto>, ICollectionBuilder<MyDataAccountDto>
{
    private readonly Faker<MyDataAccountDto> _faker = new AutoFaker<MyDataAccountDto>()
        .RuleFor(f => f.Type, faker => faker.PickRandom(AccountType.List.ToArray()));

    public static MyDataAccountDtoBuilder GivenMyAccountData() => new();

    public MyDataAccountDto Build()
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate();
    }

    public MyDataAccountDto[] Build(int count)
    {
        _faker.AssertConfigurationIsValid();
        return _faker.Generate(count).ToArray();
    }
}
