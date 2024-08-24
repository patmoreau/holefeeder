using System.Text.Json.Serialization;

namespace Holefeeder.Ui.Common.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType
{
    Checking,
    CreditCard,
    CreditLine,
    Investment,
    Loan,
    Mortgage,
    Savings
}
