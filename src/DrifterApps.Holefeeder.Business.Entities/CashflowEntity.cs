using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class CashflowEntity : BaseEntity, IOwnedEntity<CashflowEntity>, IIdentityEntity<CashflowEntity>
    {
        public DateTime EffectiveDate { get; set; }
        public decimal Amount { get; set; }
        public DateIntervalType IntervalType { get; set; }
        public int Frequency { get; set; }
        public int Recurrence { get; set; }
        public string Description { get; set; }
        public string Account { get; set; }
        public string Category { get; set; }
        public bool Inactive { get; set; }
        public IReadOnlyList<string> Tags { get; set; }
        public string Guid { get; set; }
        public string UserId { get; set; }

        public CashflowEntity(string id, DateTime effectiveDate, decimal amount, DateIntervalType intervalType, int frequency, int recurrence, string description, string account, string category, bool inactive, IEnumerable<string> tags, string guid, string userId) : base(id)
        {
            EffectiveDate = effectiveDate;
            Amount = amount;
            IntervalType = intervalType;
            Frequency = frequency;
            Recurrence = recurrence;
            Description = description;
            Account = account;
            Category = category;
            Inactive = inactive;
            Tags = ImmutableList.CreateRange(tags ?? new string[0]);
            Guid = guid;
            UserId = userId;
        }

        public CashflowEntity With(string id = null, DateTime? effectiveDate = null, decimal? amount = null, DateIntervalType? intervalType = null, int? frequency = null, int? recurrence = null, string description = null, string account = null, string category = null, bool? inactive = null, IEnumerable<string> tags = null, string guid = null, string userId = null) =>
            new CashflowEntity(
                id ?? Id,
                effectiveDate ?? EffectiveDate,
                amount ?? Amount,
                intervalType ?? IntervalType,
                frequency ?? Frequency,
                recurrence ?? Recurrence,
                description ?? Description,
                account ?? Account,
                category ?? Category,
                inactive ?? Inactive,
                ImmutableList.CreateRange(tags ?? Tags),
                guid ?? Guid,
                userId ?? UserId
            );

        public CashflowEntity WithUser(string userId) => this.With(userId: userId);

        public CashflowEntity WithId(string id) => this.With(id: id);
    }
}
