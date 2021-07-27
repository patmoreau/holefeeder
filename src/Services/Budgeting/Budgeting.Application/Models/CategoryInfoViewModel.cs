using System;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record CategoryInfoViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public CategoryType Type { get; init; }
        public string Color { get; init; }

        public CategoryInfoViewModel(Guid id, string name, CategoryType type, string color)
        {
            Id = id;
            Name = name;
            Type = type;
            Color = color;
        }
    }
}
