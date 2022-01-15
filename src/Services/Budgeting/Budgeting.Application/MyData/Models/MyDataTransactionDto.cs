using System;
using System.Collections.Immutable;
using System.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Application.MyData.Models
{
    public record MyDataTransactionDto
    {
        private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;

        public Guid Id { get; init; }
        
        public DateTime Date { get; init; }
        
        public decimal Amount { get; init; }
        
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
        
        public Guid? CashflowId { get; init; }
        
        public DateTime? CashflowDate { get; init; }
    }
}
