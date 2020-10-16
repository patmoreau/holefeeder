using System;
using DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate;

namespace DrifterApps.Holefeeder.Application.Models
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
