using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core;

internal sealed record UnexpectedError(string Code, string Description) : ResultError(Code, Description);
