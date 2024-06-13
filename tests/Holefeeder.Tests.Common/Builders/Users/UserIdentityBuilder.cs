using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Tests.Common.Builders.Users;

public class UserIdentityBuilder : FakerBuilder<UserIdentity>
{
    private UserIdentityBuilder()
    {
    }

    protected override Faker<UserIdentity> FakerRules { get; } = new Faker<UserIdentity>()
        .RuleFor(x => x.Inactive, false);

    public UserIdentityBuilder WithIdentityObjectId(string identityObjectId)
    {
        FakerRules.RuleFor(x => x.IdentityObjectId, identityObjectId);
        return this;
    }

    public static UserIdentityBuilder GivenAUserIdentity(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var builder = new UserIdentityBuilder();
        builder.FakerRules.RuleFor(x => x.UserId, user.Id);
        builder.FakerRules.RuleFor(x => x.User, user);
        builder.FakerRules.RuleFor(x => x.IdentityObjectId, f => f.Random.Hash());
        return builder;
    }
}
