using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class CategoryInfoDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public CategoryType Type { get; set; }

        public string Color { get; set; }
    }
}
