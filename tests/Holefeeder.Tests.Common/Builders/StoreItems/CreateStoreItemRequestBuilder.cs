using DrifterApps.Seeds.Testing;

using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class CreateStoreItemRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> FakerRules { get; } = new Faker<Request>()
        .CustomInstantiator(faker => new Request(faker.Random.Hash(), faker.Lorem.Paragraphs()))
        .RuleFor(x => x.Code, faker => faker.Random.Hash())
        .RuleFor(x => x.Data, faker => faker.Lorem.Paragraphs());

    public static CreateStoreItemRequestBuilder GivenACreateStoreItemRequest() => new();

    public CreateStoreItemRequestBuilder WithCode(string code)
    {
        FakerRules.RuleFor(x => x.Code, code);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoCode()
    {
        FakerRules.RuleFor(x => x.Code, string.Empty);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoData()
    {
        FakerRules.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
