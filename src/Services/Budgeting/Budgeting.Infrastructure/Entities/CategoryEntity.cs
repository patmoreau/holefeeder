using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities
{
    [Table("categories")]
    public record CategoryEntity : EntityRoot
    {
        public const string SCHEMA = "categories";

        public string Name { get; init; }

        public CategoryType Type { get; init; }

        public string Color { get; init; }

        public decimal BudgetAmount { get; init; }

        public bool Favorite { get; init; }

        public bool System { get; init; }

        [Key]
        public Guid UserId { get; init; }
    }
}
