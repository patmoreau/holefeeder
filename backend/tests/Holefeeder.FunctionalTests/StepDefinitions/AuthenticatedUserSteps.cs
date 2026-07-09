using DrifterApps.Seeds.Testing.Drivers;

using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class AuthenticatedUserSteps(IApplicationDriver applicationDriver) :
    AuthApiSteps<IUser>(applicationDriver);
