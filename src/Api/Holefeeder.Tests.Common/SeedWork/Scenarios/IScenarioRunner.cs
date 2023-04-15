// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

namespace Holefeeder.Tests.Common.SeedWork.Scenarios;

#pragma warning disable CA1716
public interface IScenarioRunner : IRunnerContext
{
    IScenarioRunner Given(string message, Action action);
    IScenarioRunner Given(string message, Func<Task> action);
    IScenarioRunner Given(Action<IStepRunner> action);
    IScenarioRunner When(string message, Action action);
    IScenarioRunner When(string message, Func<Task> action);
    IScenarioRunner When(Action<IStepRunner> action);
    IScenarioRunner Then(string message, Action action);
    IScenarioRunner Then(string message, Func<Task> action);
    IScenarioRunner Then(Action<IStepRunner> action);
    IScenarioRunner And(string message, Action action);
    IScenarioRunner And(string message, Func<Task> action);
    IScenarioRunner And(Action<IStepRunner> action);
}
