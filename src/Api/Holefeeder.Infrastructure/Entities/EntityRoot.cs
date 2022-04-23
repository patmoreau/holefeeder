using System.ComponentModel.DataAnnotations;

namespace Holefeeder.Infrastructure.Entities;

internal abstract record EntityRoot
{
    [Key]
    public Guid Id { get; init; }
}
