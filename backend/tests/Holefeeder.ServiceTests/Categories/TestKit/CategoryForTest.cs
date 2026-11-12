using Dapper;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.Categories.TestKit;

internal static class CategoryForTest
{
    private const string Insert =
        """
        INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id)
        VALUES (@Id, @Type, @Name, @Color, @Budget, @Favorite, @System, @Inactive, @UserId);
        """;

    internal static Category ACategory(
        string? name = null, CategoryType type = CategoryType.Expense, string color = "#34C759", decimal budget = 0m,
        bool favorite = false, bool system = false) =>
        new(AnId(), CategoryName.Valid(name ?? $"category-{Guid.NewGuid()}"), type, CategoryColor.Valid(color),
            Money.Valid(budget), favorite, system);

    public static async Task<Category> SavedInDbAsync(this Category category, User user, DatabaseForTest database, bool inactive = false)
    {
        var parameters = new DynamicParameters(new
        {
            Id = category.Id.Value,
            Type = category.Type.ToString(),
            Name = category.Name.Value,
            Color = category.Color.Value,
            Budget = category.Budget.Value,
            category.Favorite,
            category.System,
            Inactive = inactive,
            UserId = user.Id.Value
        });

        await using var connection = database.DataSource.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(Insert, parameters));
        return category;
    }
}
