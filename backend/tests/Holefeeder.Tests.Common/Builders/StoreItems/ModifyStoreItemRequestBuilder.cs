using DrifterApps.Seeds.Testing;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class ModifyStoreItemRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker<Request>()
        .RuleFor(x => x.Id, faker => (StoreItemId)faker.RandomGuid())
        .RuleFor(x => x.Data, faker => faker.Lorem.Paragraphs());

    public static ModifyStoreItemRequestBuilder GivenAModifyStoreItemRequest() => new();

    public ModifyStoreItemRequestBuilder WithId(StoreItemId id)
    {
        Faker.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoId()
    {
        Faker.RuleFor(x => x.Id, StoreItemId.Empty);
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
