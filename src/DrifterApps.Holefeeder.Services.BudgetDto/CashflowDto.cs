using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class CashflowDto
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        public decimal Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DateIntervalType IntervalType { get; set; }

        public int Frequency { get; set; }

        public int Recurrence { get; set; }

        public string Description { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string Category { get; set; }

        public bool Inactive { get; set; }

        public IList<string> Tags { get; set; }
    }
}
