using Holefeeder.Service.Support.Api;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Support.TestKit;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.ServiceTests.Support.Api;

public class UserMiddlewareTests
{
    private readonly UserContextAccessor _contextAccessor = new();
    private readonly UsersInMemory _users = new();
    private readonly UserMiddleware _middleware;

    private HttpContext? _contextPassedToNext;
    private User? _userSeenByNext;

    public UserMiddlewareTests() =>
        _middleware = new UserMiddleware(httpContext =>
        {
            _contextPassedToNext = httpContext;
            _userSeenByNext = _contextAccessor.Current;
            return Task.CompletedTask;
        });

    [Fact]
    public async Task Invoke_SetsNoUser_WhenNoUserInContext()
    {
        var httpContext = AnEmptyHttpContext();

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextPassedToNext.Should().BeSameAs(httpContext);
        _userSeenByNext.Should().Be(User.NoUser);
    }

    [Fact]
    public async Task Invoke_SetsNoUser_WhenUserNotFound()
    {
        var httpContext = AHttpContext();

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextPassedToNext.Should().BeSameAs(httpContext);
        _userSeenByNext.Should().Be(User.NoUser);
    }

    [Fact]
    public async Task Invoke_SetsUser_WhenUserFound()
    {
        var user = AUser();
        _users.Add(user);
        var httpContext = AHttpContext(user.IdentityObjectId.Value);

        await _middleware.InvokeAsync(httpContext, _contextAccessor, _users);

        _contextPassedToNext.Should().BeSameAs(httpContext);
        _userSeenByNext.Should().Be(user);
    }
}
