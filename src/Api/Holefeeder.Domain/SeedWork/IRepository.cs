namespace Holefeeder.Domain.SeedWork;

public interface IRepository
{
    IUnitOfWork UnitOfWork { get; }
}
