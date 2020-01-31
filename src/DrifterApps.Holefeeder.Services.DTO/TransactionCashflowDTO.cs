using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class TransactionCashflowDTO
    {
        public string Id { get; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DateIntervalType IntervalType { get; }

        public int Frequency { get; }

        public int Recurrence { get; }

        public TransactionCashflowDTO(string id, DateTime effectiveDate, DateIntervalType intervalType, int frequency, int recurrence)
        {
            Id = id;
            EffectiveDate = effectiveDate;
            IntervalType = intervalType;
            Frequency = frequency;
            Recurrence = recurrence;
        }
    }
}