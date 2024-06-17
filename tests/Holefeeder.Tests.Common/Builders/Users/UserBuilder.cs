using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.Users;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.Users;

public class UserBuilder : FakerBuilder<User>
{
    private UserBuilder()
    {
    }

    protected override Faker<User> FakerRules { get; } = new Faker<User>()
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Inactive, false)
        .RuleFor(x => x.UserIdentities, new List<UserIdentity>());

    public UserBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public static UserBuilder GivenAUser() => new();
}
