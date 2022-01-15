using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models
{
    public record MyDataCategoryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string Color { get; init; } = null!;
        public decimal BudgetAmount { get; init; }
        public bool Favorite { get; init; }
        public bool System { get; init; }
    }
}
