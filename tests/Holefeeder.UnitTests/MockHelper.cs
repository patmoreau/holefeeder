using Holefeeder.Application.UserContext;
using Holefeeder.Tests.Common.Extensions;

using Microsoft.Extensions.Logging;

namespace Holefeeder.UnitTests;

internal static class MockHelper
{
    public static ILogger<T> CreateLogger<T>() => Substitute.For<ILogger<T>>();

    internal static IUserContext CreateUserContext()
    {
        var userContextMock = Substitute.For<IUserContext>();
        userContextMock.Id.Returns(Fakerizer.RandomGuid());
        return userContextMock;
    }
}
