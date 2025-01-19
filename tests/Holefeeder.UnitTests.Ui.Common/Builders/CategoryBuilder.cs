using Bogus;

using DrifterApps.Seeds.Testing;

using Holefeeder.Ui.Common.Models;

namespace Holefeeder.UnitTests.Ui.Common.Builders;

internal sealed class CategoryBuilder : FakerBuilder<Category>
{
    protected override Faker<Category> Faker { get; } = CreateFaker<Category>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Name, f => f.Lorem.Word())
        .RuleFor(x => x.Color, f => f.Internet.Color())
        .RuleFor(x => x.BudgetAmount, f => f.Random.Decimal(min: 0))
        .RuleFor(x => x.Favorite, f => f.Random.Bool());
}
