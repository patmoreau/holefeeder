using System;
using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities
{
    public abstract record EntityRoot
    {
        // [ExplicitKey]
        [Key]
        public Guid Id { get; init; }
    }
}
