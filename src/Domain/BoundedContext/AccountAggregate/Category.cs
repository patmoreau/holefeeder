using System.Collections.Generic;
using DrifterApps.Holefeeder.Domain.SeedWork;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate
{
    public class Category : ValueObject
    {
        public Category(string name, CategoryType type, string color)
        {
            Name = name;
            Type = type;
            Color = color;
        }

        public string Name { get; }
        public CategoryType Type { get; }
        public string Color { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Type;
            yield return Color;
        }
    }
}
