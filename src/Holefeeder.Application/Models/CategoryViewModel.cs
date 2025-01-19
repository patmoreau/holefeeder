using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Models;

public record CategoryViewModel
{
    public required CategoryId Id { get; init; }
    public string Name { get; init; } = null!;
    public CategoryColor Color { get; init; }
    public Money BudgetAmount { get; init; }
    public bool Favorite { get; init; }
}
