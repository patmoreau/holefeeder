using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Application.Models;

public record CategoryInfoViewModel
{
    public CategoryInfoViewModel(Guid id, string name, CategoryType type, string color)
    {
        Id = id;
        Name = name;
        Type = type;
        Color = color;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public CategoryType Type { get; init; }
    public string Color { get; init; }
}
