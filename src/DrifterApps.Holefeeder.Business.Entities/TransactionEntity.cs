using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class TransactionEntity : BaseEntity, IIdentityEntity<TransactionEntity>, IOwnedEntity<TransactionEntity>
    {
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public string Account { get; }
        public string Category { get; }
        public string Cashflow { get; }
        public DateTime? CashflowDate { get; }
        public IReadOnlyList<string> Tags { get; }
        public string UserId { get; }

        public TransactionEntity(string id, DateTime date, decimal amount, string description, string category, string account, string cashflow, DateTime? cashflowDate, IEnumerable<string> tags, string userId = "") : base(id)
        {
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Cashflow = cashflow;
            CashflowDate = cashflowDate;
            UserId = userId;
            Tags = tags == null ? ImmutableList.Create<string>() : ImmutableList.CreateRange(tags);
        }

        public TransactionEntity With(
            string id = null,
            DateTime? date = null,
            decimal? amount = null,
            string description = null,
            string category = null,
            string account = null,
            string cashflow = null,
            DateTime? cashflowDate = null,
            string userId = null,
            IEnumerable<string> tags = null) =>
            new TransactionEntity(
                id ?? Id,
                date ?? Date,
                amount ?? Amount,
                description ?? Description,
                category ?? Category,
                account ?? Account,
                cashflow ?? Cashflow,
                cashflowDate ?? CashflowDate,
                tags ?? Tags,
                userId ?? UserId);


        public TransactionEntity WithId(string id) => With(id);

        public TransactionEntity WithUser(string userId) => With(userId: userId);
    }
}
