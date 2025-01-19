using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class ForbiddenUserSteps(IApplicationDriver applicationDriver) : AuthApiSteps<IForbiddenUser>(applicationDriver);
