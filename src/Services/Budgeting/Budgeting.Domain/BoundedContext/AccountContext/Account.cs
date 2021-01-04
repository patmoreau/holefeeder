using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext
{
    public class Account : Entity, IAggregateRoot
    {
        public Account(Guid id, AccountType type, string name, bool favorite, decimal openBalance, DateTime openDate,
            string description, bool inactive)
        {
            Id = id;
            Type = type;
            Name = name;
            Favorite = favorite;
            OpenBalance = openBalance;
            OpenDate = openDate;
            Description = description;
            Inactive = inactive;
        }

        public AccountType Type { get; }
        public string Name { get; }
        public bool Favorite { get; }
        public decimal OpenBalance { get; }
        public DateTime OpenDate { get; }
        public string Description { get; }
        public bool Inactive { get; }
    }
}
