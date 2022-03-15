using System;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models;

public record AccountInfoViewModel
{
    public AccountInfoViewModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
}
