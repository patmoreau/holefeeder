using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class CashflowDto
    {
        public string Id { get; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; }

        public decimal Amount { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DateIntervalType IntervalType { get; }

        public int Frequency { get; }

        public int Recurrence { get; }

        public string Description { get; }

        [Required]
        public string Account { get; }

        [Required]
        public string Category { get; }

        public bool Inactive { get; }

        public IReadOnlyList<string> Tags { get; }

        public CashflowDto(string id, DateTime effectiveDate, decimal amount, DateIntervalType intervalType, int frequency, int recurrence, string description, string category, string account, bool inactive, IReadOnlyList<string> tags)
        {
            Id = id;
            EffectiveDate = effectiveDate;
            Amount = amount;
            IntervalType = intervalType;
            Frequency = frequency;
            Recurrence = recurrence;
            Description = description;
            Category = category;
            Account = account;
            Inactive = inactive;
            Tags = new List<string>(tags);
        }
    }
}
