namespace Holefeeder.Domain.Features.Categories;

public sealed record Category : IAggregateRoot
{
    private readonly Guid _id;
    private readonly string _name = string.Empty;
    private readonly Guid _userId;

    public required Guid Id
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

    public required CategoryType Type { get; init; }

    public required string Name
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

    public required Guid UserId
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
        string description, Guid userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Name = name,
            UserId = userId,
            BudgetAmount = budgetAmount
        };
}
