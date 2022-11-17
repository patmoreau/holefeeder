using AutoBogus;

using Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem;

using static Holefeeder.Application.Features.StoreItems.Commands.ModifyStoreItem.ModifyStoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal sealed class ModifyStoreItemRequestFactory : AutoFaker<Request>
{
    public ModifyStoreItemRequestFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Data, faker => faker.Random.Words());
    }
}
