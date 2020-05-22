using System;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class AccountDetailEntity : BaseEntity, IIdentityEntity<AccountDetailEntity>
    {
        public AccountType Type { get; }
        public string Name { get; }
        public int TransactionCount { get; }
        public decimal Balance { get; }
        public DateTime? Updated { get; }
        public string Description { get; }
        public bool Favorite { get; }
        public bool Inactive { get; }

        public AccountDetailEntity(string id, AccountType type, string name, int transactionCount, decimal balance, DateTime? updated, string description, bool favorite, bool inactive) : base(id)
        {
            Type = type;
            Name = name;
            TransactionCount = transactionCount;
            Balance = balance;
            Updated = updated;
            Description = description;
            Favorite = favorite;
            Inactive = inactive;
        }

        public AccountDetailEntity With(string id = null, AccountType? type = null, string name = null, int? transactionCount = null, decimal? balance = null, DateTime? updated = null, string description = null, bool? favorite = null, bool? inactive = null) =>
            new AccountDetailEntity(
                id ?? Id,
                type ?? Type,
                name ?? Name,
                transactionCount ?? TransactionCount,
                balance ?? Balance,
                updated ?? Updated,
                description ?? Description,
                favorite ?? Favorite,
                inactive ?? Inactive);

        public AccountDetailEntity WithId(string id) => this.With(id);
    }
}
