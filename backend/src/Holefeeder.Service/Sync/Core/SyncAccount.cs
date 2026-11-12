using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal sealed record SyncAccount(
    Id Id,
    AccountType Type,
    string Name,
    Money OpenBalance,
    DateOnly OpenDate,
    string Description,
    bool Favorite,
    bool Inactive) : ISyncRecord
{
    private const int NameMaxLength = 100;

    public SyncTable Table => SyncTable.Accounts;

    public Result<ISyncRecord> Validated() =>
        SyncRecordErrors.Validated(this,
            (!string.IsNullOrWhiteSpace(Name) && Name.Length <= NameMaxLength, SyncRecordErrors.AccountNameInvalid),
            (OpenDate != default, SyncRecordErrors.AccountOpenDateRequired));
}

internal sealed record SyncAccountPatch(
    AccountType? Type = null,
    string? Name = null,
    Money? OpenBalance = null,
    DateOnly? OpenDate = null,
    string? Description = null,
    bool? Favorite = null,
    bool? Inactive = null) : ISyncPatch
{
    public SyncTable Table => SyncTable.Accounts;

    public Result<ISyncRecord> Create(Id id) =>
        new SyncAccount(
            id,
            Type ?? AccountType.Checking,
            Name ?? string.Empty,
            OpenBalance ?? Money.Zero,
            OpenDate ?? default,
            Description ?? string.Empty,
            Favorite ?? false,
            Inactive ?? false).Validated();

    public Result<ISyncRecord> ApplyTo(ISyncRecord existing)
    {
        var account = (SyncAccount)existing;
        return (account with
        {
            Type = Type ?? account.Type,
            Name = Name ?? account.Name,
            OpenBalance = OpenBalance ?? account.OpenBalance,
            OpenDate = OpenDate ?? account.OpenDate,
            Description = Description ?? account.Description,
            Favorite = Favorite ?? account.Favorite,
            Inactive = Inactive ?? account.Inactive
        }).Validated();
    }

    public ISyncRecord? KeptOnDelete(ISyncRecord existing) => null;
}
