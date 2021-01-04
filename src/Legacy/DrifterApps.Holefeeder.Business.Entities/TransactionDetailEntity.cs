using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class TransactionDetailEntity : BaseEntity, IIdentityEntity<TransactionDetailEntity>
    {
        public DateTime Date { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public AccountInfoEntity Account { get; }
        public CategoryInfoEntity Category { get; }
        public CashflowInfoEntity Cashflow { get; }
        public IReadOnlyList<string> Tags { get; }

        public TransactionDetailEntity(string id, DateTime date, decimal amount, string description, CategoryInfoEntity category, AccountInfoEntity account, CashflowInfoEntity cashflow, IEnumerable<string> tags) : base(id)
        {
            Date = date;
            Amount = amount;
            Description = description;
            Category = category;
            Account = account;
            Cashflow = cashflow;
            Tags = tags == null ? ImmutableList.Create<string>() : ImmutableList.CreateRange(tags);
        }

        public TransactionDetailEntity With(
            string id = null,
            DateTime? date = null,
            decimal? amount = null,
            string description = null,
            CategoryInfoEntity category = null,
            AccountInfoEntity account = null,
            CashflowInfoEntity cashflow = null,
            IEnumerable<string> tags = null) =>
            new TransactionDetailEntity(
                id ?? Id,
                date ?? Date,
                amount ?? Amount,
                description ?? Description,
                category ?? Category,
                account ?? Account,
                cashflow ?? Cashflow,
                tags ?? Tags
                );

        public TransactionDetailEntity WithId(string id) => With(id);
    }
}
