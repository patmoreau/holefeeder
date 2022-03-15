using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

[Table("store_items")]
public record StoreItemEntity
{
    [Key] public Guid Id { get; init; }

    public string Code { get; init; } = null!;

    public string Data { get; init; } = null!;

    [Key] public Guid UserId { get; init; }
}
