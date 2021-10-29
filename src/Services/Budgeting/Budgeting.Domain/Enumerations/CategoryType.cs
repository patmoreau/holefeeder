using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Converters;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Enumerations
{
    [JsonConverter(typeof(EnumerationJsonConverter<CategoryType>))]
    public abstract class CategoryType : Enumeration
    {
        public static readonly CategoryType Expense = new ExpenseCategoryType(1, nameof(Expense));
        public static readonly CategoryType Gain = new GainCategoryType(2, nameof(Gain));
        private CategoryType(int id, string name) : base(id, name) { }

        public virtual int Multiplier => 1;

        private class ExpenseCategoryType : CategoryType
        {
            public ExpenseCategoryType(int id, string name) : base(id, name) { }

            public override int Multiplier => -1;
        }

        private class GainCategoryType : CategoryType
        {
            public GainCategoryType(int id, string name) : base(id, name) { }
        }
    }
}
