using System;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business.Entities
{
    public class AccountEntity : BaseEntity, IOwnedEntity<AccountEntity>, IIdentityEntity<AccountEntity>
    {
        public AccountType Type { get; }
        public string Name { get; }
        public bool Favorite { get; }
        public decimal OpenBalance { get; }
        public DateTime OpenDate { get; }
        public string Description { get; }
        public bool Inactive { get; }
        public string Guid { get; }
        public string UserId { get; }

        public AccountEntity(string id, AccountType type, string name, bool favorite, decimal openBalance, DateTime openDate, string description, bool inactive, string guid, string userId) : base(id)
        {
            Type = type;
            Name = name;
            Favorite = favorite;
            OpenBalance = openBalance;
            OpenDate = openDate;
            Description = description;
            Inactive = inactive;
            Guid = guid;
            UserId = userId;
        }

        public AccountEntity With(string id = null, AccountType? type = null, string name = null, bool? favorite = null, decimal? openBalance = null, DateTime? openDate = null, string description = null, bool? inactive = null, string guid = null, string userId = null) =>
            new AccountEntity(
                id ?? Id,
                type ?? Type,
                name ?? Name,
                favorite ?? Favorite,
                openBalance ?? OpenBalance,
                openDate ?? OpenDate,
                description ?? Description,
                inactive ?? Inactive,
                guid ?? Guid,
                userId ?? UserId);

        public AccountEntity WithUser(string userId) => this.With(userId: userId);

        public AccountEntity WithId(string id) => this.With(id: id);
    }
}