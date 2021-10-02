using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Schemas
{
    public class CategorySchema : BaseSchema, IOwnedSchema
    {
        public const string SCHEMA = "categories";
        
        public string Name { get; set; }

        public CategoryType Type { get; set; }

        public string Color { get; set; }

        public decimal BudgetAmount { get; set; }

        public bool Favorite { get; set; }

        public bool System { get; set; }

        public Guid UserId { get; set; }
    }
}
