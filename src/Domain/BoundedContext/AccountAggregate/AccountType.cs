using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DrifterApps.Holefeeder.Domain.SeedWork;
using DrifterApps.Holefeeder.Domain.Exceptions;

namespace DrifterApps.Holefeeder.Domain.AggregatesModel.AccountAggregate
{
    public class AccountType : Enumeration
    {
        public static AccountType Checking = new AccountType(1, nameof(Checking));
        public static AccountType CreditCard = new AccountType(2, nameof(CreditCard));
        public static AccountType CreditLine = new AccountType(3, nameof(CreditLine));
        public static AccountType Investment = new AccountType(4, nameof(Investment));
        public static AccountType Loan = new AccountType(5, nameof(Loan));
        public static AccountType Mortgage = new AccountType(6, nameof(Mortgage));
        public static AccountType Savings = new AccountType(7, nameof(Savings));

        private AccountType(int id, string name) : base(id, name)
        {
        }

        public int GetMultiplier()
        {
            if(Equals(this, Checking)) return 1;
            if(Equals(this, Investment)) return 1;
            if(Equals(this, Savings)) return 1;
            if(Equals(this, Loan)) return -1;
            if(Equals(this, Mortgage)) return -1;
            if(Equals(this, CreditCard)) return -1;
            if(Equals(this, CreditLine)) return -1;
            return -1;
        }
    }
}
