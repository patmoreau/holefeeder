using Bogus;

using Holefeeder.Service.StoreItems.Core;

namespace Holefeeder.ServiceTests.StoreItems.TestKit;

internal static class CodeForTest
{
    private static readonly Faker Faker = new();

    public static Code ACode(string? code = null) => Code.Valid(code ?? Faker.Random.String2(10));
}
