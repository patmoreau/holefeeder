using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext
{
    public record Account : IAggregateRoot
    {
        private readonly Guid _id;
        private readonly string _name = string.Empty;
        private readonly DateTime _openDate;
        private readonly Guid _userId;

        public Guid Id
        {
            get=>_id;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(Id)} is required");
                }

                _id = value;
            }
        }
        public AccountType Type { get; init; }
        public string Name
        {
            get=>_name;
            init
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > 255)
                {
                    throw new HolefeederDomainException($"{nameof(Name)} must be from 1 to 255 characters");
                }

                _name = value;
            }
        }

        public bool Favorite { get; init; }
        public decimal OpenBalance { get; init; }
        public DateTime OpenDate
        {
            get=>_openDate;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(OpenDate)} is required");
                }

                _openDate = value;
            }
        }

        public string Description { get; init; }
        public bool Inactive { get; init; }
        public Guid UserId
        {
            get => _userId;
            init
            {
                if (value.Equals(default))
                {
                    throw new HolefeederDomainException($"{nameof(UserId)} is required");
                }

                _userId = value;
            }
        }

        public IReadOnlyList<Guid> Cashflows { get; init; }

        public Account()
        {
            Cashflows = ImmutableList<Guid>.Empty;
        }
        
        public static Account Create(AccountType type, string name, decimal openBalance, DateTime openDate,
            string description, Guid userId)
            => new()
            {
                Id = Guid.NewGuid(),
                Type = type,
                Name = name,
                Favorite = false,
                OpenBalance = openBalance,
                OpenDate = openDate,
                Description = description,
                Inactive = false,
                UserId = userId,
                Cashflows = ImmutableList<Guid>.Empty
            };

        public Account Close()
        {
            if (Inactive)
            {
                throw new HolefeederDomainException("Account already closed");
            }

            if (Cashflows.Any())
            {
                throw new HolefeederDomainException("Account has active cashflows");
            }

            return this with {Inactive = true};
        }
    }
}
