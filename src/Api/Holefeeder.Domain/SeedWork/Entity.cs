using MediatR;

namespace Holefeeder.Domain.SeedWork;

public abstract record Entity
{
    private readonly List<INotification> _domainEvents = new();
    public virtual Guid Id { get; init; }

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

    public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
