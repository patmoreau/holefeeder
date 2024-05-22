using DrifterApps.Seeds.Testing;

using Holefeeder.Tests.Common.Extensions;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class ModifyStoreItemRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.RandomGuid(), faker.Lorem.Paragraphs()))
        .RuleFor(x => x.Id, faker => faker.RandomGuid())
        .RuleFor(x => x.Data, faker => faker.Lorem.Paragraphs());

    public static ModifyStoreItemRequestBuilder GivenAModifyStoreItemRequest() => new();

    public ModifyStoreItemRequestBuilder WithId(Guid id)
    {
        FakerRules.RuleFor(x => x.Id, id);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoId()
    {
        FakerRules.RuleFor(x => x.Id, Guid.Empty);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithData(string data)
    {
        FakerRules.RuleFor(x => x.Data, data);
        return this;
    }

    public ModifyStoreItemRequestBuilder WithNoData()
    {
        FakerRules.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
