using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class AccountDto
    {
        public string Id { get; set; }

        [Required]
        public AccountType Type { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Favorite { get; set; }

        public decimal OpenBalance { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; set; }

        public string Description { get; set; }

        public bool Inactive { get; set; }
    }
}
