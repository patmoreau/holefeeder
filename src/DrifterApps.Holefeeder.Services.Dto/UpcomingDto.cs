using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class UpcomingDto
    {
        public string Id { get; }
        
        public DateTime Date { get; }
        
        public decimal Amount { get; }
        
        public string Description { get; }

        public IList<string> Tags { get; }
        
        public CategoryInfoDto Category { get; }
        
        public AccountInfoDto Account { get; }

        public UpcomingDto(string id, DateTime date, decimal amount, string description, CategoryInfoDto category, AccountInfoDto account, IEnumerable<string> tags)
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
