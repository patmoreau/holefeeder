using AutoBogus;

using Holefeeder.Domain.Features.Categories;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal sealed class CategoryFactory : AutoFaker<Category>
{
    private const decimal BUDGET_AMOUNT_MAX = 100m;

    public CategoryFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Name, faker => faker.Random.String2(1, 100));
        RuleFor(x => x.Type, faker => faker.PickRandom(CategoryType.List.ToArray()));
        RuleFor(x => x.Color, faker => faker.Random.String2(0, 25));
        RuleFor(x => x.BudgetAmount, faker => faker.Finance.Amount(max: BUDGET_AMOUNT_MAX));
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.System, faker => faker.Random.Bool());
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
