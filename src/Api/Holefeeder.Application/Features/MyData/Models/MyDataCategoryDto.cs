using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Application.Features.MyData.Models;

public record MyDataCategoryDto
{
    public Guid Id { get; init; }
    public CategoryType Type { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Color { get; init; } = null!;
    public decimal BudgetAmount { get; init; }
    public bool Favorite { get; init; }
    public bool System { get; init; }
}
