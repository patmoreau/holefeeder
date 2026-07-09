using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Tests.Common.Builders.Users;

internal class UserIdentityBuilder : FakerBuilder<UserIdentity>
{
    private static readonly User User = new UserBuilder().Build();

    protected override Faker<UserIdentity> Faker { get; } = CreateUninitializedFaker<UserIdentity>()
        .RuleFor(x => x.IdentityObjectId, faker => faker.Random.Hash())
        .RuleFor(x => x.UserId, _ => User.Id)
        .RuleFor(x => x.User, _ => User)
        .RuleFor(x => x.Inactive, false);

    public UserIdentityBuilder WithIdentityObjectId(string identityObjectId)
    {
        Faker.RuleFor(x => x.IdentityObjectId, identityObjectId);
        return this;
    }

    public static UserIdentityBuilder GivenAUserIdentity(User user)
    {
        var builder = new UserIdentityBuilder();
        builder.Faker.RuleFor(x => x.UserId, user.Id);
        builder.Faker.RuleFor(x => x.User, user);
        return builder;
    }
}
