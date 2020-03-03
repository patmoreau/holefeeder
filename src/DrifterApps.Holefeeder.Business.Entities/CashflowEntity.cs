using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class CashflowEntity : BaseEntity, IOwnedEntity<CashflowEntity>, IIdentityEntity<CashflowEntity>
    {
        public DateTime EffectiveDate { get; }
        public decimal Amount { get; }
        public DateIntervalType IntervalType { get; }
        public int Frequency { get; }
        public int Recurrence { get; }
        public string Description { get; }
        public string Account { get; }
        public string Category { get; }
        public bool Inactive { get; }
        public IReadOnlyList<string> Tags { get; }
        public string Guid { get; }
        public string UserId { get; }

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
            Tags = ImmutableList.CreateRange(tags ?? Array.Empty<string>());
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
