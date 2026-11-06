using Holefeeder.Service.Support.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Support.TestKit;

using Microsoft.AspNetCore.Http;

using NSubstitute;

namespace Holefeeder.ServiceTests.Support.Api;

public class UserMiddlewareTests
{
    private static readonly RequestDelegate RequestDelegate = Substitute.For<RequestDelegate>();
    private readonly UserMiddleware _middleware = new(RequestDelegate);
    private readonly UserContextAccessor _contextAccessor = new();
    private readonly UsersInMemory _users = new();

    [Fact]
    public async Task Invoke_SetsNoUser_WhenNoUserInContext()
    {
        var httpContext = AnEmptyHttpContext();

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextAccessor.Current.Should().Be(User.NoUser);
        await RequestDelegate.Received().Invoke(httpContext);
    }

    [Fact]
    public async Task Invoke_SetsNoUser_WhenUserNotFound()
    {
        var httpContext = AHttpContext();

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextAccessor.Current.Should().Be(User.NoUser);
        await RequestDelegate.Received().Invoke(httpContext);
    }

    [Fact]
    public async Task Invoke_SetsUser_WhenUserFound()
    {
        var user = AUser();
        _users.Add(user);
        var httpContext = AHttpContext(user.IdentityObjectId.Value);

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextAccessor.Current.Should().Be(user);
        await RequestDelegate.Received().Invoke(httpContext);
    }
}
