using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.Dto
{
    public class CategoryDto
    {
        public string Id { get; }

        [Required]
        public string Name { get; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType Type { get; }

        public string Color { get; }

        public decimal BudgetAmount { get; }

        public bool Favorite { get; }

        public bool System { get; }

        public CategoryDto(string id, string name, CategoryType type, string color, decimal budgetAmount, bool favorite, bool system)
        {
            Id = id;
            Name = name;
            Type = type;
            Color = color;
            BudgetAmount = budgetAmount;
            Favorite = favorite;
            System = system;
        }
    }
}
