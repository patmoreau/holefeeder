using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Common.Extensions
{
    public static class AccountTypeExtensions
    {
        public static int GetMultiplier(this AccountType self) =>
            self switch
            {
                AccountType.Checking => 1,
                AccountType.Investment => 1,
                AccountType.Savings => 1,
                AccountType.Loan => -1,
                AccountType.Mortgage => -1,
                AccountType.CreditCard => -1,
                AccountType.CreditLine => -1,
                _ => -1
            };
    }
}
