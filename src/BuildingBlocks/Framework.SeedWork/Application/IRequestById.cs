using System;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public interface IRequestById
{
    public Guid Id { get; init; }
}
