using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DrifterApps.Holefeeder.Application.Models
{
    public class UpcomingViewModel
    {
        public Guid Id { get; }
        
        public DateTime Date { get;  }
        
        public decimal Amount { get; }
        
        public string Description { get; }

        public IReadOnlyList<string> Tags { get; }
        
        public CategoryInfoViewModel Category { get; }
        
        public AccountInfoViewModel Account { get; }

        public UpcomingViewModel(Guid id, DateTime date, decimal amount, string description,
            CategoryInfoViewModel category, AccountInfoViewModel account, IEnumerable<string> tags)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Tags = ImmutableList.Create(tags.ToArray());
        }
    }
}
