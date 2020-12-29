using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class TransactionDetailDto
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public AccountInfoDto Account { get; set; }

        public CategoryInfoDto Category { get; set; }

        public CashflowInfoDto Cashflow { get; set; }

        public IList<string> Tags { get; set;  }
    }
}
