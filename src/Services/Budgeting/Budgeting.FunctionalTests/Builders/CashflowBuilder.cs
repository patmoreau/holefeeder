using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;

namespace DrifterApps.Holefeeder.Budgeting.FunctionalTests.Builders
{
    public class CashflowBuilder
    {
        private static readonly object _locker = new();
        private static int _seed = 1;
        private static readonly List<CashflowEntity> _cashflows = new();

        public static IReadOnlyList<CashflowEntity> Cashflows => _cashflows;

        private Guid _id;
        private string _description;
        private decimal _amount;
        private DateTime _effectiveDate;
        private DateIntervalType _type;
        private int _frequency;
        private int _recurrence;
        private Guid _account;
        private Guid _category;
        private bool _inactive;
        private Guid _userId;

        public static CashflowBuilder Create(Guid id)
        {
            lock (_locker)
            {
                return new CashflowBuilder(id, _seed);
            }
        }

        private CashflowBuilder(Guid id, int seed)
        {
            _id = id;
            _amount = (Convert.ToDecimal(seed) * 100m) + (Convert.ToDecimal(seed) / 100m);
            _description = $"Cashflow{seed}";
            _effectiveDate = (new DateTime(2020, 1, 1)).AddDays(seed);
            _type = DateIntervalType.Weekly;
            _frequency = 2;
            _recurrence = 0;
            _account = Guid.Empty;
            _category = Guid.Empty;
            _inactive = false;
            _userId = Guid.Empty;
        }
        
        public CashflowBuilder OfAmount(decimal amount)
        {
            _amount = amount;

            return this;
        }
        
        public CashflowBuilder OfType(DateIntervalType type)
        {
            _type = type;

            return this;
        }
        
        public CashflowBuilder WithFrequency(int frequency)
        {
            _frequency = frequency;

            return this;
        }
        
        public CashflowBuilder ForAccount(Guid account)
        {
            _account = account;

            return this;
        }
        
        public CashflowBuilder ForCategory(Guid category)
        {
            _category = category;

            return this;
        }

        public CashflowBuilder IsInactive()
        {
            _inactive = true;

            return this;
        }

        public CashflowBuilder ForUser(Guid userId)
        {
            _userId = userId;

            return this;
        }

        public void Build()
        {
            var schema = new CashflowEntity()
            {
                Id = _id,
                Amount = _amount,
                EffectiveDate = _effectiveDate,
                Description = _description,
                IntervalType = _type,
                Frequency = _frequency,
                Recurrence = _recurrence,
                UserId = _userId,
                Inactive = _inactive,
                AccountId = _account,
                CategoryId = _category
            };

            lock (_locker)
            {
                _seed++;
                _cashflows.Add(schema);
            }
        }
    }
}
