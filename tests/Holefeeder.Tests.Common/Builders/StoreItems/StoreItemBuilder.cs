using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class StoreItemBuilder : FakerBuilder<StoreItem>
{
    protected override Faker<StoreItem> FakerRules { get; } = new Faker<StoreItem>()
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Code, faker => faker.Random.String2(1, 100))
        .RuleFor(x => x.Data, faker => faker.Random.Words())
        .RuleFor(x => x.UserId, faker => faker.RandomGuid());

    public static StoreItemBuilder GivenAStoreItem() => new();

    public StoreItemBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);

        return this;
    }

    public StoreItemBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);

        return this;
    }

    public StoreItemBuilder WithCode(string code)
    {
        FakerRules.RuleFor(x => x.Code, code);

        return this;
    }

    public StoreItemBuilder ForUser(Guid userId)
    {
        FakerRules.RuleFor(x => x.UserId, userId);

        return this;
    }

    public StoreItemBuilder ForNoUser()
    {
        FakerRules.RuleFor(x => x.UserId, Guid.Empty);

        return this;
    }
}
