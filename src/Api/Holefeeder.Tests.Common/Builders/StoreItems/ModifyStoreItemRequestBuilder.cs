using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class ModifyStoreItemRequestBuilder : RootBuilder<Request>
{
    public static ModifyStoreItemRequestBuilder GivenAModifyStoreItemRequest() => new();

    public ModifyStoreItemRequestBuilder WithId(Guid id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithData(string data)
    {
        Faker.RuleFor(x => x.Data, data);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoData()
    {
        Faker.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
