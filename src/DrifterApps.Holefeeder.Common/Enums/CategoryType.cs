using System.ComponentModel;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.Common.Enums
{
    public enum CategoryType
    {
        [EnumMember(Value=CategoryTypes.EXPENSE)]
        [Description(CategoryTypes.EXPENSE)]
        Expense,
        [EnumMember(Value=CategoryTypes.GAIN)]
        [Description(CategoryTypes.GAIN)]
        Gain
    }
}