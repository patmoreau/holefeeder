using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category : IAggregateRoot<CategoryId>
{
    // The transfer feature resolves its categories by name, so these are part of the
    // domain rather than of whichever endpoint happens to need them.
    public const string TransferOutName = "Transfer Out";
    public const string TransferInName = "Transfer In";

    // Blue-grey: these categories are plumbing, so they read as neutral next to the
    // ones the user picks colours for.
    public static CategoryColor SystemColor => CategoryColor.Create("#607D8B").Value;

    private Category(CategoryId id, CategoryType type, string name, CategoryColor color, Money budgetAmount, UserId userId)
    {
        Id = id;
        Type = type;
        Name = name;
        Color = color;
        BudgetAmount = budgetAmount;
        UserId = userId;
    }

    public CategoryId Id { get; }

    public CategoryType Type { get; }

    public string Name { get; }

    public CategoryColor Color { get; }

    public bool Favorite { get; private init; }

    public bool System { get; private init; }

    public bool Inactive { get; private init; }

    public Money BudgetAmount { get; }

    public UserId UserId { get; }
}

public sealed record CategoryId : StronglyTypedId<CategoryId>;
