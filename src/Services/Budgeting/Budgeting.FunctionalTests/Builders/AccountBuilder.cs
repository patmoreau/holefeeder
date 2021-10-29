using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders
{
    public class AccountBuilder
    {
        private static readonly object _locker = new();
        private static int _seed = 1;
        private static readonly List<AccountEntity> _accounts = new();

        public static IReadOnlyList<AccountEntity> Accounts => _accounts;

        private Guid _id;
        private string _name;
        private string _description;
        private decimal _openBalance;
        private DateTime _openDate;
        private AccountType _type;
        private bool _favorite;
        private bool _inactive;
        private Guid _userId;

        public static AccountBuilder Create(Guid id)
        {
            lock (_locker)
            {
                return new AccountBuilder(id, _seed);
            }
        }

        private AccountBuilder(Guid id, int seed)
        {
            _id = id;
            _name = $"Account{seed}";
            _description = $"Description{seed}";
            _openBalance = (Convert.ToDecimal(seed) * 100m) + (Convert.ToDecimal(seed) / 100m);
            _openDate = (new DateTime(2019, 1, 1)).AddDays(seed);
            _type = AccountType.Checking;
            _favorite = false;
            _inactive = false;
            _userId = Guid.NewGuid();
        }

        public AccountBuilder OfType(AccountType type)
        {
            _type = type;

            return this;
        }

        public AccountBuilder IsFavorite()
        {
            _favorite = true;

            return this;
        }

        public AccountBuilder IsInactive()
        {
            _inactive = true;

            return this;
        }

        public AccountBuilder ForUser(Guid userId)
        {
            _userId = userId;

            return this;
        }

        public void Build()
        {
            lock (_locker)
            {
                _seed++;
                _accounts.Add(BuildNew());
            }
        }

        public AccountEntity BuildSingle() => BuildNew();

        private AccountEntity BuildNew() => new()
        {
            Id = _id,
            Name = _name,
            Description = _description,
            Type = _type,
            UserId = _userId,
            Favorite = _favorite,
            Inactive = _inactive,
            OpenBalance = _openBalance,
            OpenDate = _openDate
        };
    }
}
