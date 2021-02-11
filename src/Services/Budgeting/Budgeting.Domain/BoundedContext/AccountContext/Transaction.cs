using System;
using System.Collections.Generic;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext
{
    public class Transaction : ValueObject
    {
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public Category Category { get; }
        public IReadOnlyList<string> Tags { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Date;
            yield return Amount;
            yield return Description;
            yield return Category;
            yield return Tags;
        }
    }
}
