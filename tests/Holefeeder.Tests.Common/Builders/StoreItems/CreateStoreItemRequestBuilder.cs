using DrifterApps.Seeds.Testing;

using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class CreateStoreItemRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker()
        .RuleFor(x => x.Code, faker => faker.Random.Hash())
        .RuleFor(x => x.Data, faker => faker.Lorem.Paragraphs());

    public static CreateStoreItemRequestBuilder GivenACreateStoreItemRequest() => new();

    public CreateStoreItemRequestBuilder WithCode(string code)
    {
        Faker.RuleFor(x => x.Code, code);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoCode()
    {
        Faker.RuleFor(x => x.Code, string.Empty);
        return this;
    }

    public CreateStoreItemRequestBuilder WithNoData()
    {
        Faker.RuleFor(x => x.Data, string.Empty);
        return this;
    }
}
