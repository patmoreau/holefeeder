using System.Text.Json;

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.Converters;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.UserContext;

internal class UserContext : IUserContext
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new DateOnlyJsonConverter()
        }
    };

    private UserId? _userId;
    private UserSettings? _userSettings;

    public UserId Id => _userId ?? UserId.Empty;

    public UserSettings Settings => _userSettings ?? UserSettings.Default;

    internal async Task InitializeAsync(IHttpUserContext httpUserContext, BudgetingContext context)
    {
        // Load User ID
        _userId = await context.Users
            .Where(user =>
                user.UserIdentities.Any(identity => identity.IdentityObjectId == httpUserContext.IdentityObjectId))
            .Select(user => user.Id)
            .SingleOrDefaultAsync();

        if (_userId is null)
        {
            _userId = UserId.Empty;
            _userSettings = UserSettings.Default;
            return;
        }

        // Load User Settings
        var settings = await context.StoreItems
            .Where(e => e.UserId == _userId && e.Code == StoreItem.CodeSettings)
            .FirstOrDefaultAsync();

        if (settings is null)
        {
            _userSettings = UserSettings.Default;
            return;
        }

        var userSettings = JsonSerializer.Deserialize<UserSettings>(settings.Data, Options);
        _userSettings = userSettings ?? UserSettings.Default;
    }
}
