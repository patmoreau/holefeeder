using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Tests.Common.Builders.Users;

internal class UserSettingsBuilder : FakerBuilder<UserSettings>
{
    protected override Faker<UserSettings> Faker { get; } = CreatePrivateFaker<UserSettings>()
        .RuleFor(x => x.EffectiveDate, faker => faker.Date.PastDateOnly())
        .RuleFor(x => x.IntervalType, faker => faker.PickRandom<DateIntervalType>(DateIntervalType.List))
        .RuleFor(x => x.Frequency, 1);

    public UserSettingsBuilder WithEffectiveDate(DateOnly effectiveDate)
    {
        Faker.RuleFor(x => x.EffectiveDate, _ => effectiveDate);
        return this;
    }

    public UserSettingsBuilder WithIntervalType(DateIntervalType intervalType)
    {
        Faker.RuleFor(x => x.IntervalType, _ => intervalType);
        return this;
    }

    public static UserSettingsBuilder GivenAUserSettings() => new();
}
