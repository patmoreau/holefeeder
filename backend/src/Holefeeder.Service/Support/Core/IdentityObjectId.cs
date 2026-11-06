using System.Security.Claims;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Support.Core;

internal record IdentityObjectId
{
    public string Value { get; }

    public static Result<IdentityObjectId> Create(ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

        if (value is null)
        {
            return IdentityObjectIdErrors.Invalid;
        }

        return new IdentityObjectId(value);
    }

    public static IdentityObjectId Valid(string value) => new(value);


    public static IdentityObjectId Empty => new(string.Empty);

    private IdentityObjectId(string value) => Value = value;
}

internal static class IdentityObjectIdErrors
{
    public const string CodeInvalid = "identity-object-id-invalid";

    public static ResultError Invalid => new(CodeInvalid, "IdentityObjectId is invalid.");
}
