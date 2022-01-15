using System;
using System.Collections.Immutable;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models
{
    public record MyDataCashflowDto
    {
        private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;

        public Guid Id { get; init; }
        
        public DateTime EffectiveDate { get; init; }
        
        public decimal Amount { get; init; }
        
        public DateIntervalType IntervalType { get; set; } = null!;

        public int Frequency { get; set; }

        public int Recurrence { get; set; }

        public string Description { get; init; } = null!;

        public ImmutableArray<string> Tags
        {
            get => _tags;
            init
            {
                _tags = ImmutableArray.Create(value.ToArray());
            }
        }

        public Guid CategoryId { get; init; }
        
        public Guid AccountId { get; init; }
    }
}
