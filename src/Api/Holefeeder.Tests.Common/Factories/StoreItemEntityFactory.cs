using AutoBogus;

using Bogus.Extensions;

using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class StoreItemEntityFactory : AutoFaker<StoreItemEntity>
{
    public StoreItemEntityFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Code, faker => faker.Random.String2(minLength: 1, maxLength: 100));
        RuleFor(x => x.Data, faker => faker.Random.Words());
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
