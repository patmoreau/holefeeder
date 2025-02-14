using System.Text.Json.Serialization;

namespace Holefeeder.Ui.Shared.Models;

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
