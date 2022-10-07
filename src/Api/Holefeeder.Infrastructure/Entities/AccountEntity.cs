using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Infrastructure.Entities;

[Table("accounts")]
internal record AccountEntity : EntityRoot
{
    public AccountType Type { get; init; } = null!;

    public string Name { get; init; } = null!;

    public bool Favorite { get; init; }

    public decimal OpenBalance { get; init; }

    public DateTime OpenDate { get; init; }

    public string Description { get; init; } = null!;

    public bool Inactive { get; init; }

    [Key] public Guid UserId { get; init; }
}
