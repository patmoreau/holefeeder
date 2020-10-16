using System;
using System.Collections.Generic;
using System.Linq;
using DrifterApps.Holefeeder.Domain.Exceptions;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate
{
    public class CategoryType : Enumeration
    {
        public static CategoryType Expense = new CategoryType(1, nameof(Expense));
        public static CategoryType Gain = new CategoryType(2, nameof(Gain));
        private CategoryType(int id, string name) : base(id, name)
        {
        }

        private static IEnumerable<CategoryType> List()
        {
            return new[] {Expense, Gain};
        }

        public int GetMultiplier()
        {
            if (Equals(this, Expense)) return -1;
            if (Equals(this, Gain)) return 1;
            return -1;
        }
    }
}
