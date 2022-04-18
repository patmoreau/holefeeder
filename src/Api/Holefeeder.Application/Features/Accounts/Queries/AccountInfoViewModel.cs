namespace Holefeeder.Application.Features.Accounts.Queries;

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
