using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class UpcomingEntity
    {
        public string Id { get; }
        public DateTime Date { get; }
        public DateTime? LastPaidDate { get; }
        public DateTime? LastCashflowDate { get; }
        public DateIntervalType IntervalType { get; }
        public int Frequency { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public IList<string> Tags { get; }
        public CategoryInfoEntity Category { get; }
        public AccountInfoEntity Account { get; }

        public UpcomingEntity(
            string id,
            DateTime date,
            DateTime? lastPaidDate,
            DateTime? lastCashflowDate,
            DateIntervalType intervalType,
            int frequency,
            decimal amount,
            string description,
            IEnumerable<string> tags,
            CategoryInfoEntity category,
            AccountInfoEntity account)
        {
            Id = id;
            Date = date;
            LastPaidDate = lastPaidDate;
            LastCashflowDate = lastCashflowDate;
            IntervalType = intervalType;
            Frequency = frequency;
            Amount = amount;
            Description = description;
            Tags = new List<string>(tags);
            Category = category;
            Account = account;
        }

        public UpcomingEntity With(
            string id = null,
            DateTime? date = null,
            DateTime? lastPaidDate = null,
            DateTime? lastCashflowDate = null,
            DateIntervalType? intervalType = null,
            int? frequency = null,
            decimal? amount = null,
            string description = null,
            IEnumerable<string> tags = null,
            CategoryInfoEntity category = null,
            AccountInfoEntity account = null)
        => new UpcomingEntity(
            id??this.Id,
            date??this.Date,
            lastPaidDate??this.LastPaidDate,
            lastCashflowDate??this.LastCashflowDate,
            intervalType??this.IntervalType,
            frequency??this.Frequency,
            amount??this.Amount,
            description??this.Description,
            tags??this.Tags,
            category??this.Category,
            account??this.Account);
    }
}