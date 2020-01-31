using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class AccountDTO
    {
        public string Id { get; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType Type { get; }

        [Required]
        public string Name { get; }

        public bool Favorite { get; }

        public decimal OpenBalance { get; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; }

        public string Description { get; }

        public bool Inactive { get; }

        public AccountDTO(string id, AccountType type, string name, bool favorite, decimal openBalance, DateTime openDate, string description, bool inactive)
        {
            Id = id;
            Type = type;
            Name = name;
            Favorite = favorite;
            OpenBalance = openBalance;
            OpenDate = openDate;
            Description = description;
            Inactive = inactive;
        }
    }
}