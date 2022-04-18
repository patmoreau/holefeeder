using System.ComponentModel.DataAnnotations;

namespace Holefeeder.Infrastructure.Entities;

public abstract record EntityRoot
{
    [Key]
    public Guid Id { get; init; }
}
