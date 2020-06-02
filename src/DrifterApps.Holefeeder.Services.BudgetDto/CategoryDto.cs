using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class CategoryDto
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public CategoryType Type { get; set; }

        public string Color { get; set; }

        public decimal BudgetAmount { get; set; }

        public bool Favorite { get; set; }

        public bool System { get; set; }
    }
}
