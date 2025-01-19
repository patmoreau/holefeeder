using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Tests.Common.Builders.Users;

internal class UserBuilder : FakerBuilder<User>
{
    protected override Faker<User> Faker { get; } = CreatePrivateFaker<User>()
        .RuleFor(x => x.Id, faker => (UserId)faker.Random.Guid())
        .RuleFor(x => x.Inactive, false)
        .RuleFor(x => x.UserIdentities, new List<UserIdentity>());

    public UserBuilder WithId(UserId userid)
    {
        Faker.RuleFor(x => x.Id, _ => userid);
        return this;
    }

    public static UserBuilder GivenAUser() => new();
}
