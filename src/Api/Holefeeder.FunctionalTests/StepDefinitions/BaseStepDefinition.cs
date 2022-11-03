using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public abstract class BaseStepDefinition
{
    private readonly HttpClientDriver _httpClientDriver;
    private readonly List<Action> _steps = new();

    protected BaseStepDefinition(HttpClientDriver httpClientDriver)
    {
        _httpClientDriver = httpClientDriver;
    }

    protected void AddStep(Action action)
    {
        _steps.Add(action);
    }

    protected void AddStep(Func<Task> action)
    {
        AddStep(() => action().Wait());
    }

    protected void ExecuteSteps()
    {
        var steps = _steps.ToList();
        _steps.Clear();

        foreach (var step in steps)
        {
            step();
        }
    }
    public Guid WithCreatedId()
    {
        ExecuteSteps();

        var result = _httpClientDriver.DeserializeContent<Created>();

        result.Should().NotBeNull();

        return result!.Id;
    }

    public TResult WithResultAs<TResult>()
    {
        ExecuteSteps();

        var result = _httpClientDriver.DeserializeContent<TResult>();
        result.Should().NotBeNull();

        return result!;
    }

    private record Created(Guid Id);
}
