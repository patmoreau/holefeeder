using DrifterApps.Seeds.Testing;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.Tests.Common.Builders.Categories;

internal class CategoryBuilder : FakerBuilder<Category>
{
    protected override Faker<Category> Faker { get; } = CreatePrivateFaker<Category>()
            .RuleFor(x => x.Id, faker => (CategoryId)faker.RandomGuid())
            .RuleFor(x => x.Type, faker => faker.PickRandom<CategoryType>(CategoryType.List))
            .RuleFor(x => x.Name, faker => faker.Lorem.Word() + $" #{faker.IndexFaker}")
            .RuleFor(x => x.Color, _ => CategoryColorBuilder.Create().Build())
            .RuleFor(x => x.Favorite, faker => faker.Random.Bool())
            .RuleFor(x => x.System, false)
            .RuleFor(x => x.BudgetAmount, _ => MoneyBuilder.Create().Build())
            .RuleFor(x => x.UserId, faker => faker.RandomGuid());

    public static CategoryBuilder GivenACategory() => new();

    public static CategoryBuilder GivenATransferInCategory() => new CategoryBuilder()
        .WithName(Transfer.CategoryToName)
        .OfType(CategoryType.Gain);

    public static CategoryBuilder GivenATransferOutCategory() => new CategoryBuilder()
        .WithName(Transfer.CategoryFromName)
        .OfType(CategoryType.Expense);

    public CategoryBuilder WithId(Guid id)
    {
        Faker.RuleFor(f => f.Id, id);
        return this;
    }

    public CategoryBuilder WithNoId()
    {
        Faker.RuleFor(f => f.Id, CategoryId.Empty);
        return this;
    }

    public CategoryBuilder OfType(CategoryType type)
    {
        Faker.RuleFor(f => f.Type, type);
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        Faker.RuleFor(f => f.Name, name);
        return this;
    }

    public CategoryBuilder ForUser(UserId userId)
    {
        Faker.RuleFor(f => f.UserId, userId);
        return this;
    }

    public CategoryBuilder ForNoUser()
    {
        Faker.RuleFor(f => f.UserId, Guid.Empty);
        return this;
    }

    public CategoryBuilder IsFavorite()
    {
        Faker.RuleFor(f => f.Favorite, true);
        return this;
    }

    public CategoryBuilder IsNotFavorite()
    {
        Faker.RuleFor(f => f.Favorite, false);
        return this;
    }
}
