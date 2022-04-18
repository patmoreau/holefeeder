using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Accounts;

public class Category : ValueObject
{
    public Category(string name, CategoryType type, string color)
    {
        Name = name;
        Type = type;
        Color = color;
    }

    public string Name { get; }
    public CategoryType Type { get; }
    public string Color { get; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Type;
        yield return Color;
    }
}
