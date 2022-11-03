using FluentAssertions.Execution;

using Holefeeder.FunctionalTests.Drivers;

using Xunit.Abstractions;

namespace Holefeeder.FunctionalTests.Features;

public abstract class BaseScenario<T> : BaseScenario where T : BaseScenario<T>
{
    private readonly List<Func<Task>> _tasks = new();
    private readonly ITestOutputHelper _testOutputHelper;
    private int _taskCount;

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    protected T Given(Action action)
    {
        return Given(string.Empty, action);
    }

    protected T Given(string message, Action action)
    {
        AddTask(nameof(Given), message, action);
        return (T)this;
    }

    protected T Given(Func<Task> action)
    {
        return Given(string.Empty, action);
    }

    protected T Given(string message, Func<Task> action)
    {
        AddTask(nameof(Given), message, action);

        return (T)this;
    }

    protected T When(Action action)
    {
        return When(string.Empty, action);
    }

    protected T When(string message, Action action)
    {
        AddTask(nameof(When), message, action);

        return (T)this;
    }

    protected T When(Func<Task> action)
    {
        return When(string.Empty, action);
    }

    protected T When(string message, Func<Task> action)
    {
        AddTask(nameof(When), message, action);

        return (T)this;
    }

    protected T Then(Action action)
    {
        return Then(string.Empty, action);
    }

    protected T Then(string message, Action action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using var scope = new AssertionScope();
            return Task.Run(action);
        });

        return (T)this;
    }

    protected T Then(Func<Task> action)
    {
        return Then(string.Empty, action);
    }

    protected T Then(string message, Func<Task> action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using var scope = new AssertionScope();
            return action();
        });

        return (T)this;
    }

    protected async Task RunScenarioAsync()
    {
        var tasks = _tasks.ToArray();
        _tasks.Clear();

        foreach (var task in tasks)
        {
            await task();
        }
    }

    private void AddTask(string command, string message, Action action)
    {
        AddTask(command, message, () => Task.Run(action));
    }

    private void AddTask(string command, string message, Func<Task> action)
    {
        _taskCount++;
        var text = string.IsNullOrWhiteSpace(message) ? $"task #{_taskCount}" : message;
        _tasks.Add(() => Task.Run(() => _testOutputHelper.WriteLine($"{command} {text}")));
        _tasks.Add(() => Task.Run(action));
    }
}
