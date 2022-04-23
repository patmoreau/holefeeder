using AutoBogus;

using Bogus.Extensions;

using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.FunctionalTests.Infrastructure.Factories;

internal sealed class StoreItemEntityFactory : AutoFaker<StoreItemEntity>
{
    public StoreItemEntityFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Code, faker => faker.Random.Word().ClampLength(100));
        RuleFor(x => x.Data, faker => faker.Random.Words());
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
