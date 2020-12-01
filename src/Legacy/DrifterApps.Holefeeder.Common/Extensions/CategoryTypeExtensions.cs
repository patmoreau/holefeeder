using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Common.Extensions
{
    public static class CategoryTypeExtensions
    {
        public static int GetMultiplier(this CategoryType self) =>
            self switch
            {
                CategoryType.Expense => -1,
                CategoryType.Gain => 1,
                _ => 1
            };
    }
}
