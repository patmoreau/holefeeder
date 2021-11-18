using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

[Table("categories")]
public record CategoryEntity : EntityRoot
{
    public string Name { get; init; } = null!;

    public CategoryType Type { get; init; } = null!;

    public string Color { get; init; } = null!;

    public decimal BudgetAmount { get; init; }

    public bool Favorite { get; init; }

    public bool System { get; init; }

    [Key]
    public Guid UserId { get; init; }
}
