using System;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public class AccountViewModel
    {
        public Guid Id { get; }

        public AccountType Type { get; }

        public string Name { get; }

        public int TransactionCount { get; }

        public decimal Balance { get; }

        public DateTime? Updated { get; }

        public string Description { get; }

        public bool Favorite { get; }

        [JsonConstructor]
        public AccountViewModel(Guid id, AccountType type, string name,
            int transactionCount, decimal balance, DateTime? updated, string description,
            bool favorite)
        {
            Id = id;
            Type = type;
            Name = name;
            TransactionCount = transactionCount;
            Balance = balance;
            Updated = updated;
            Description = description;
            Favorite = favorite;
        }
    }
}
