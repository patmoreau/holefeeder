using Holefeeder.Tests.Common.SeedWork;
using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal class CreateStoreItemRequestBuilder : RootBuilder<Request>
{
    public static CreateStoreItemRequestBuilder GivenACreateStoreItemRequest() => new CreateStoreItemRequestBuilder();

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
