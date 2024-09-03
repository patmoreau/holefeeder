using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Tests.Common.Builders;

internal class MoneyBuilder
{
    private readonly Faker _faker = new();

    private MoneyBuilder()
    {
    }

    public Money Build() => Money.Create(_faker.Finance.Amount()).Value;

    public static MoneyBuilder Create() => new MoneyBuilder();
}
