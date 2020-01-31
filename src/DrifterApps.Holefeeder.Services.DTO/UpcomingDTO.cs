using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class UpcomingDTO
    {
        public string Id { get; }
        
        public DateTime Date { get; }
        
        public decimal Amount { get; }
        
        public string Description { get; }

        public IList<string> Tags { get; }
        
        public CategoryInfoDTO Category { get; }
        
        public AccountInfoDTO Account { get; }

        public UpcomingDTO(string id, DateTime date, decimal amount, string description, CategoryInfoDTO category, AccountInfoDTO account, IEnumerable<string> tags)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Tags = tags == null ? null : new List<string>(tags);
        }
    }
}