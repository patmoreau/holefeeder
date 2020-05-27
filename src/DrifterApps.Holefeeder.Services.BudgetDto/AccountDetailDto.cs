using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class AccountDetailDto
    {
        public string Id { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountType Type { get; set; }

        [Required]
        public string Name { get; set; }

        public int TransactionCount { get; set; }

        public decimal Balance { get; set; }

        public DateTime? Updated { get; set; }

        public string Description { get; set; }

        public bool Favorite { get; set; }

        public bool Inactive { get; set; }
    }
}
