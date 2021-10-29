using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record CashflowViewModel
    {
        private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;

        public Guid Id { get; init; }
        
        public DateTime EffectiveDate { get; init; }
        
        public decimal Amount { get; init; }
        
        public DateIntervalType IntervalType { get; set; }

        public int Frequency { get; set; }

        public int Recurrence { get; set; }

        public string Description { get; init; }

        public ImmutableArray<string> Tags
        {
            get => _tags;
            init
            {
                _tags = value == null ? ImmutableArray<string>.Empty : ImmutableArray.Create(value.ToArray());
            }
        }

        public CategoryInfoViewModel Category { get; init; }
        
        public AccountInfoViewModel Account { get; init; }
    }
}
