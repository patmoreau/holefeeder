using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Application.UseCases;

public static class SyncErrors
{
    public const string CodeTypeInvalid = $"{nameof(PowerSync)}.{nameof(CodeTypeInvalid)}";

    public static ResultError TypeInvalid(string type) => new(CodeTypeInvalid, $"Type '{type}' is invalid");
}
