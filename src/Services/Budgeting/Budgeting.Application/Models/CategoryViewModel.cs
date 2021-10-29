using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record CategoryViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Color { get; init; }
        public decimal BudgetAmount { get; init; }
        public bool Favorite { get; init; }
    }
}
