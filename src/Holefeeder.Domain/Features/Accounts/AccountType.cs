using System.Text.Json.Serialization;

using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

using Holefeeder.Domain.Converters;

// ReSharper disable UnusedMember.Global

namespace Holefeeder.Domain.Features.Accounts;

[JsonConverter(typeof(SmartEnumCamelCaseConverter<AccountType, int>))]
[SmartEnumStringComparer(StringComparison.InvariantCultureIgnoreCase)]
public abstract class AccountType : SmartEnum<AccountType>
{
    public static readonly AccountType Checking = new DebitAccountType(1, nameof(Checking));
    public static readonly AccountType CreditCard = new CreditAccountType(2, nameof(CreditCard));
    public static readonly AccountType CreditLine = new CreditAccountType(3, nameof(CreditLine));
    public static readonly AccountType Investment = new DebitAccountType(4, nameof(Investment));
    public static readonly AccountType Loan = new CreditAccountType(5, nameof(Loan));
    public static readonly AccountType Mortgage = new CreditAccountType(6, nameof(Mortgage));
    public static readonly AccountType Savings = new DebitAccountType(7, nameof(Savings));

    private AccountType(int id, string name) : base(name, id)
    {
    }

    public abstract int Multiplier { get; }

    private sealed class CreditAccountType(int id, string name) : AccountType(id, name)
    {
        public override int Multiplier => -1;
    }

    private sealed class DebitAccountType(int id, string name) : AccountType(id, name)
    {
        public override int Multiplier => 1;
    }
}
