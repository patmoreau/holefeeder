using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Holefeeder.Domain.Features.Categories;

[JsonConverter(typeof(SmartEnumNameConverter<CategoryType, int>))]
public abstract class CategoryType : SmartEnum<CategoryType>
{
    public static readonly CategoryType Expense = new ExpenseCategoryType(1, nameof(Expense));
    public static readonly CategoryType Gain = new GainCategoryType(2, nameof(Gain));

    private CategoryType(int id, string name) : base(name, id)
    {
    }

    public virtual int Multiplier => 1;

    public virtual bool IsExpense => false;

    private sealed class ExpenseCategoryType : CategoryType
    {
        public ExpenseCategoryType(int id, string name) : base(id, name)
        {
        }

        public override int Multiplier => -1;
        public override bool IsExpense => true;
    }

    private sealed class GainCategoryType : CategoryType
    {
        public GainCategoryType(int id, string name) : base(id, name)
        {
        }
        public override bool IsExpense => false;
    }
}
