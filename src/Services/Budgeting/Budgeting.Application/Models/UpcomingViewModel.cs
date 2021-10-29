using System;
using System.Collections.Immutable;
using System.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record UpcomingViewModel
    {
        private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;
        
        public Guid Id { get; init; }
        
        public DateTime Date { get; init; }
        
        public decimal Amount { get; init; }
        
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
