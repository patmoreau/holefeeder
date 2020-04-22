using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class TransactionDto
    {
        public string Id { get; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; }

        public decimal Amount { get; }

        public string Description { get; }

        [Required]
        public string Account { get; }

        [Required]
        public string Category { get; }

        public string Cashflow { get; }

        [DataType(DataType.Date)]
        public DateTime? CashflowDate { get; }

        public IList<string> Tags { get; }

        public TransactionDto(string id, DateTime date, decimal amount, string description, string category, string account, string cashflow, DateTime? cashflowDate, IList<string> tags)
        {
            Id = id;
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Cashflow = cashflow;
            CashflowDate = cashflowDate;
            Tags = tags == null ? null : new List<string>(tags);
        }
    }
}
