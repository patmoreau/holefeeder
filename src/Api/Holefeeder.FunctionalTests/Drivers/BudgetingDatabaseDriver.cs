using Holefeeder.Application.Context;
using Holefeeder.Tests.Common.SeedWork.Drivers;

namespace Holefeeder.FunctionalTests.Drivers;

public sealed class BudgetingDatabaseDriver : DbContextDriver
{
    public BudgetingDatabaseDriver(BudgetingContext context) => DbContext = context;

    protected override BudgetingContext DbContext { get; }
}
