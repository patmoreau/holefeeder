using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class TransactionDetailDto
    {
        public string Id { get; }

        public DateTime Date { get; }

        public decimal Amount { get; }

        public string Description { get; }

        public AccountInfoDto Account { get; }

        public CategoryInfoDto Category { get; }

        public CashflowInfoDto Cashflow { get; }

        public IList<string> Tags { get; }

        public TransactionDetailDto(string id, DateTime date, decimal amount, string description, CategoryInfoDto category, AccountInfoDto account, CashflowInfoDto cashflow, IList<string> tags)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Cashflow = cashflow;
            Tags = tags == null ? null : new List<string>(tags);
        }
    }
}
