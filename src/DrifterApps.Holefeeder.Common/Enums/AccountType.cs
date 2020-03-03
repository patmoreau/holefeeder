using System.ComponentModel;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.Common.Enums
{
    public enum AccountType
    {
        [EnumMember(Value=AccountTypes.CHECKING)]
        [Description(AccountTypes.CHECKING)]
        Checking,
        [EnumMember(Value=AccountTypes.CREDITCARD)]
        [Description(AccountTypes.CREDITCARD)]
        CreditCard,
        [EnumMember(Value=AccountTypes.CREDITLINE)]
        [Description(AccountTypes.CREDITLINE)]
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