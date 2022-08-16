using AutoBogus;

using Bogus.Extensions;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Entities;

namespace Holefeeder.Tests.Common.Factories;

internal sealed class CategoryEntityFactory : AutoFaker<CategoryEntity>
{
    private const decimal BUDGET_AMOUNT_MAX = 100m;

    public CategoryEntityFactory()
    {
        RuleFor(x => x.Id, faker => faker.Random.Guid());
        RuleFor(x => x.Name, faker => faker.Random.String2(minLength: 1, maxLength: 100));
        RuleFor(x => x.Type, faker => faker.PickRandom(CategoryType.List.ToArray()));
        RuleFor(x => x.Color, faker => faker.Random.String2(minLength: 0, maxLength: 25));
        RuleFor(x => x.BudgetAmount, faker => faker.Finance.Amount(max: BUDGET_AMOUNT_MAX));
        RuleFor(x => x.Favorite, faker => faker.Random.Bool());
        RuleFor(x => x.System, faker => faker.Random.Bool());
        RuleFor(x => x.UserId, faker => faker.Random.Guid());
    }
}
