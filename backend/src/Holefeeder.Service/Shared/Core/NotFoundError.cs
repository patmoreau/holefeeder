using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core;

internal sealed record NotFoundError(string Code) : ResultError(Code);
