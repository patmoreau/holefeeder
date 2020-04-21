using System.ComponentModel;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.Common.Enums
{
    public enum AccountType
    {
        [EnumMember(Value=AccountTypes.CHECKING)]
        [Description(AccountTypes.CHECKING)]
        Checking,
        [EnumMember(Value=AccountTypes.CREDIT_CARD)]
        [Description(AccountTypes.CREDIT_CARD)]
        CreditCard,
        [EnumMember(Value=AccountTypes.CREDIT_LINE)]
        [Description(AccountTypes.CREDIT_LINE)]
        CreditLine,
        [EnumMember(Value=AccountTypes.INVESTMENT)]
        [Description(AccountTypes.INVESTMENT)]
        Investment,
        [EnumMember(Value=AccountTypes.LOAN)]
        [Description(AccountTypes.LOAN)]
        Loan,
        [EnumMember(Value=AccountTypes.MORTGAGE)]
        [Description(AccountTypes.MORTGAGE)]
        Mortgage,
        [EnumMember(Value=AccountTypes.SAVINGS)]
        [Description(AccountTypes.SAVINGS)]
        Savings
    }
}