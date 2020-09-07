using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate
{
    public class Account : Entity, IAggregateRoot
    {
        public Account(Guid id, AccountType type, string name, bool favorite, decimal openBalance, DateTime openDate,
            string description, bool inactive, IList<Transaction> transactions) : base(id)
        {
            Type = type;
            Name = name;
            Favorite = favorite;
            OpenBalance = openBalance;
            OpenDate = openDate;
            Description = description;
            Inactive = inactive;
            Transactions = transactions;
        }

        public AccountType Type { get; }
        public string Name { get; }
        public bool Favorite { get; }
        public decimal OpenBalance { get; }
        public DateTime OpenDate { get; }
        public string Description { get; }
        public bool Inactive { get; }
        public IList<Transaction> Transactions { get; }
    }
}
