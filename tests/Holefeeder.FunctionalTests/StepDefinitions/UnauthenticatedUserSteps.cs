using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UnauthenticatedUserSteps(IApplicationDriver applicationDriver) :
    AuthApiSteps<IUnauthenticatedUser>(applicationDriver);
