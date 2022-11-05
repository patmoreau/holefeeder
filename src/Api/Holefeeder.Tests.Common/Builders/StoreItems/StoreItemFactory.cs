using AutoBogus;

using Holefeeder.Application.Domain.StoreItem;
using Holefeeder.Domain.Features.StoreItem;

namespace Holefeeder.Tests.Common.Builders.StoreItems;

internal sealed class StoreItemFactory : AutoFaker<StoreItem>
{
    public StoreItemFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Code, faker => faker.Random.String2(1, 100));
        RuleFor(x => x.Data, faker => faker.Random.Words());
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
