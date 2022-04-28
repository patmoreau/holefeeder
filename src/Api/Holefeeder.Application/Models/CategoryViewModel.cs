namespace Holefeeder.Application.Models;

public record CategoryViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Color { get; init; } = null!;
    public decimal BudgetAmount { get; init; }
    public bool Favorite { get; init; }
}
