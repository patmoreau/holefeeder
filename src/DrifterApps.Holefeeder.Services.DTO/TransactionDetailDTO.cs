using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class TransactionDetailDTO
    {
        public string Id { get; }

        public DateTime Date { get; }

        public decimal Amount { get; }

        public string Description { get; }

        public AccountInfoDTO Account { get; }

        public CategoryInfoDTO Category { get; }

        public CashflowInfoDTO Cashflow { get; }

        public IList<string> Tags { get; }

        public TransactionDetailDTO(string id, DateTime date, decimal amount, string description, CategoryInfoDTO category, AccountInfoDTO account, CashflowInfoDTO cashflow, IList<string> tags)
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
