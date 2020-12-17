using System;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public class CategoryInfoViewModel
    {
        public Guid Id { get; }

        public string Name { get; }

        public CategoryType Type { get; }

        public string Color { get; }

        [JsonConstructor]
        public CategoryInfoViewModel(Guid id, string name, CategoryType type, string color) => 
            (Id, Name, Type, Color) = (id, name, type, color ?? String.Empty);
    }
}
