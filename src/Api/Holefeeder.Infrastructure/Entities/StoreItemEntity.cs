using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holefeeder.Infrastructure.Entities;

[Table("store_items")]
internal record StoreItemEntity
{
    [Key] public Guid Id { get; init; }

    public string Code { get; init; } = null!;

    public string Data { get; init; } = null!;

    [Key] public Guid UserId { get; init; }
}
