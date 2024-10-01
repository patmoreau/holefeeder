using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class StoreItemBuilder : FakerBuilder<StoreItem>
{
    protected override Faker<StoreItem> Faker { get; } = CreatePrivateFaker()
        .RuleFor(x => x.Id, StoreItemId.New)
        .RuleFor(x => x.Code, faker => faker.Random.String2(1, 100))
        .RuleFor(x => x.Data, faker => faker.Random.Words())
        .RuleFor(x => x.UserId, faker => faker.RandomGuid());

    public static StoreItemBuilder GivenAStoreItem() => new();

    public StoreItemBuilder WithId(StoreItemId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public StoreItemBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, StoreItemId.Empty);
        return this;
    }

    public StoreItemBuilder WithCode(string code)
    {
        Faker.RuleFor(x => x.Code, code);
        return this;
    }

    public StoreItemBuilder ForUser(UserId userId)
    {
        Faker.RuleFor(x => x.UserId, userId);
        return this;
    }

    public StoreItemBuilder ForNoUser()
    {
        Faker.RuleFor(x => x.UserId, UserId.Empty);
        return this;
    }
}
