using DrifterApps.Seeds.Testing;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class StoreItemBuilder : FakerBuilder<StoreItem>
{
    protected override Faker<StoreItem> Faker { get; } = new Faker<StoreItem>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.Code, faker => faker.Random.String2(1, 100))
        .RuleFor(x => x.Data, faker => faker.Random.Words())
        .RuleFor(x => x.UserId, faker => faker.Random.Guid());

    public static StoreItemBuilder GivenAStoreItem() => new();

    public StoreItemBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);

        return this;
    }

    public StoreItemBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);

        return this;
    }

    public StoreItemBuilder WithCode(string code)
    {
        Faker.RuleFor(x => x.Code, code);

        return this;
    }

    public StoreItemBuilder ForUser(Guid userId)
    {
        Faker.RuleFor(x => x.UserId, userId);

        return this;
    }

    public StoreItemBuilder ForNoUser()
    {
        Faker.RuleFor(x => x.UserId, Guid.Empty);

        return this;
    }
}
