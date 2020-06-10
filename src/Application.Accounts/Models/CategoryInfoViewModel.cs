using System;
using DrifterApps.Holefeeder.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Application.Models
{
    public class CategoryInfoViewModel
    {
        public Guid Id { get; }

        public string Name { get; }

        public CategoryType Type { get; }

        public string Color { get; }

        public CategoryInfoViewModel(Guid id, string name, CategoryType type, string color)
        {
            Id = id;
            Name = name;
            Type = type;
            Color = color;
        }
    }
}
