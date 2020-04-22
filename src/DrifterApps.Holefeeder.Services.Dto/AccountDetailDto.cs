using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class AccountDetailDto
    {
        public string Id { get; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType Type { get; }

        [Required]
        public string Name { get; }

        public int TransactionCount { get; }

        public decimal Balance { get; }

        public DateTime? Updated { get; }

        public string Description { get; }

        public bool Favorite { get; }

        public bool Inactive { get; }

        public AccountDetailDto(string id, AccountType type, string name, int transactionCount, decimal balance, DateTime? updated, string description, bool favorite, bool inactive)
        {
            Id = id;
            Type = type;
            Name = name;
            TransactionCount = transactionCount;
            Balance = balance;
            Updated = updated;
            Description = description;
            Favorite = favorite;
            Inactive = inactive;
        }
    }
}
