using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

/// <summary>
///     Steps for a caller who is authenticated but has never been registered, which is
///     how every user starts out.
/// </summary>
internal sealed class UnregisteredUserSteps(IApplicationDriver applicationDriver) : ApiSteps<IUnregisteredUser>(applicationDriver)
{
    internal void GetsTheCurrentUser(IStepRunner runner) => runner.Execute(() => Api.GetCurrentUserAsync());

    internal void Registers(IStepRunner runner) => runner.Execute(() => Api.RegisterUserAsync());
}
