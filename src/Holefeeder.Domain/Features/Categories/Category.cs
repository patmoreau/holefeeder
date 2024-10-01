using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Domain.Features.Categories;

public sealed partial record Category : IAggregateRoot<CategoryId>
{
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

    public Money BudgetAmount { get; }

    public UserId UserId { get; }
}

public sealed record CategoryId : StronglyTypedId<CategoryId>;
