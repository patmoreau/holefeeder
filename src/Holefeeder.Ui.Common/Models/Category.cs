namespace Holefeeder.Ui.Common.Models;

public class Category
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string Color { get; init; } = string.Empty;
    public decimal BudgetAmount { get; init; }
    public bool Favorite { get; init; }
}
