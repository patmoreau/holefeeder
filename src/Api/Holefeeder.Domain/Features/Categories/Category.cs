namespace Holefeeder.Domain.Features.Categories;

public sealed record Category : Entity, IAggregateRoot
{
    private readonly Guid _id;
    private readonly string _name = string.Empty;
    private readonly Guid _userId;

    public Category(Guid id, CategoryType type, string name, Guid userId)
    {
        Id = id;
        Type = type;
        Name = name;
        UserId = userId;
    }

    public override Guid Id
    {
        get => _id;
        init
        {
            if (value.Equals(default))
            {
                throw new CategoryDomainException($"{nameof(Id)} is required");
            }

            _id = value;
        }
    }

    public CategoryType Type { get; init; }

    public string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
            {
                throw new CategoryDomainException($"{nameof(Name)} must be from 1 to 255 characters");
            }

            _name = value;
        }
    }

    public string Color { get; init; } = string.Empty;

    public bool Favorite { get; init; }

    public bool System { get; init; }

    public decimal BudgetAmount { get; init; }

    public Guid UserId
    {
        get => _userId;
        init
        {
            if (value.Equals(default))
            {
                throw new CategoryDomainException($"{nameof(UserId)} is required");
            }

            _userId = value;
        }
    }

    public static Category Create(CategoryType type, string name, decimal budgetAmount,
        string description, Guid userId)
    {
        return new Category(Guid.NewGuid(), type, name, userId) {BudgetAmount = budgetAmount};
    }
}
