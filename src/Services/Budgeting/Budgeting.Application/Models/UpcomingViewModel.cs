using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public class UpcomingViewModel
    {
        public Guid Id { get; }
        
        public DateTime Date { get;  }
        
        public decimal Amount { get; }
        
        public string Description { get; }

        public ImmutableArray<string> Tags { get; }
        
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
            Tags = tags == null ? ImmutableArray<string>.Empty : ImmutableArray.Create(tags.ToArray());
        }

        [JsonConstructor]
        public UpcomingViewModel(Guid id, DateTime date, decimal amount, string description,
            CategoryInfoViewModel category, AccountInfoViewModel account, ImmutableArray<string> tags) =>
            (Id, Date, Amount, Description, Category, Account, Tags) =
            (id, date, amount, description, category, account, tags);
    }
}
