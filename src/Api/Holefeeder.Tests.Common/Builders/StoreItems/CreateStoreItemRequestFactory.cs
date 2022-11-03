using AutoBogus;

using static Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal sealed class CreateStoreItemRequestFactory : AutoFaker<Request>
{
    public CreateStoreItemRequestFactory()
    {
        RuleFor(x => x.Code, faker => faker.Random.String2(1, 100));
        RuleFor(x => x.Data, faker => faker.Random.Words());
    }
}
