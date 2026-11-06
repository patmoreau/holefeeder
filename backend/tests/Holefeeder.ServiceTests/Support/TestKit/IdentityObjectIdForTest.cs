using Bogus;

using Holefeeder.Service.Support.Core;

namespace Holefeeder.ServiceTests.Support.TestKit;

internal static class IdentityObjectIdForTest
{
    private static readonly Faker Faker = new();

    public static IdentityObjectId AnIdentityObjectId() => IdentityObjectId.Valid(Faker.Random.AlphaNumeric(10));
}
