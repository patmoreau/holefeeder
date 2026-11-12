namespace Holefeeder.Service.Categories.Api.ListCategories;

internal sealed record CategoryResponse(Guid Id, string Name, string Type, string Color, decimal BudgetAmount, bool Favorite, bool System);
