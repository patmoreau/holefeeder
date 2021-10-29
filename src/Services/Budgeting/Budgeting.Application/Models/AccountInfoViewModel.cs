using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public record AccountInfoViewModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public AccountInfoViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
