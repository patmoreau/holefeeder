namespace DrifterApps.Holefeeder.Framework.SeedWork.Domain;

public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
