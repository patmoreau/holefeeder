using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal sealed record SyncCategory(
    Id Id,
    CategoryType Type,
    string Name,
    string Color,
    Money BudgetAmount,
    bool Favorite,
    bool System,
    bool Inactive) : ISyncRecord
{
    private const int NameMaxLength = 255;

    public SyncTable Table => SyncTable.Categories;

    public Result<ISyncRecord> Validated() =>
        SyncRecordErrors.Validated(this,
            (!string.IsNullOrWhiteSpace(Name) && Name.Length <= NameMaxLength, SyncRecordErrors.CategoryNameInvalid));
}

internal sealed record SyncCategoryPatch(
    CategoryType? Type = null,
    string? Name = null,
    string? Color = null,
    Money? BudgetAmount = null,
    bool? Favorite = null,
    bool? System = null,
    bool? Inactive = null) : ISyncPatch
{
    public SyncTable Table => SyncTable.Categories;

    public Result<ISyncRecord> Create(Id id) =>
        new SyncCategory(
            id,
            Type ?? CategoryType.Expense,
            Name ?? string.Empty,
            Color ?? string.Empty,
            BudgetAmount ?? Money.Zero,
            Favorite ?? false,
            System ?? false,
            Inactive ?? false).Validated();

    public Result<ISyncRecord> ApplyTo(ISyncRecord existing)
    {
        var category = (SyncCategory)existing;
        return (category with
        {
            Type = Type ?? category.Type,
            Name = Name ?? category.Name,
            Color = Color ?? category.Color,
            BudgetAmount = BudgetAmount ?? category.BudgetAmount,
            Favorite = Favorite ?? category.Favorite,
            System = System ?? category.System,
            Inactive = Inactive ?? category.Inactive
        }).Validated();
    }

    public ISyncRecord? KeptOnDelete(ISyncRecord existing) => (SyncCategory)existing with { Inactive = true };
}
