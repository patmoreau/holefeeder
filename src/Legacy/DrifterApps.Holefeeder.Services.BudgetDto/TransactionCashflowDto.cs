using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class TransactionCashflowDto
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        public DateIntervalType IntervalType { get; set; }

        public int Frequency { get; set; }

        public int Recurrence { get; set; }
    }
}
