using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class TransactionDto
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string Category { get; set; }

        public string Cashflow { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CashflowDate { get; set; }

        public IList<string> Tags { get; set; }
    }
}
