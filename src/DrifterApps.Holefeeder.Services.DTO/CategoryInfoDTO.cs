using System.Text.Json.Serialization;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class CategoryInfoDTO
    {
        public string Id { get; }

        public string Name { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CategoryType Type { get; }

        public string Color { get; }

        public CategoryInfoDTO(string id, string name, CategoryType type, string color)
        {
            Id = id;
            Name = name;
            Type = type;
            Color = color;
        }
    }
}