using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Converters;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Enumerations
{
    [JsonConverter(typeof(EnumerationJsonConverter<AccountType>))]
    public abstract class AccountType : Enumeration
    {
        public static readonly AccountType Checking = new DebitAccountType(1, nameof(Checking));
        public static readonly AccountType CreditCard = new CreditAccountType(2, nameof(CreditCard));
        public static readonly AccountType CreditLine = new CreditAccountType(3, nameof(CreditLine));
        public static readonly AccountType Investment = new DebitAccountType(4, nameof(Investment));
        public static readonly AccountType Loan = new CreditAccountType(5, nameof(Loan));
        public static readonly AccountType Mortgage = new CreditAccountType(6, nameof(Mortgage));
        public static readonly AccountType Savings = new DebitAccountType(7, nameof(Savings));

        private AccountType(int id, string name) : base(id, name)
        {
        }
        
        public abstract int Multiplier { get; }

        private class CreditAccountType : AccountType
        {
            public CreditAccountType(int id, string name) : base(id, name) { }

            public override int Multiplier => -1;
        }
        
        private class DebitAccountType : AccountType
        {
            public DebitAccountType(int id, string name) : base(id, name) { }

            public override int Multiplier => 1;
        }
    }
}
