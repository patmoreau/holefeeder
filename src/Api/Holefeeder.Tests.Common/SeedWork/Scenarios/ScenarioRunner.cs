using FluentAssertions;
using FluentAssertions.Execution;
using Xunit.Abstractions;

namespace Holefeeder.Tests.Common.SeedWork.Scenarios;

internal class ScenarioRunner : IScenarioRunner, IStepRunner
{
    private readonly IDictionary<string, object> _context = new Dictionary<string, object>();
    private readonly List<(string Command, string Description, Func<Task> Task)> _tasks = new();
    private readonly ITestOutputHelper _testOutputHelper;
    private string _stepCommand = string.Empty;

    private ScenarioRunner(string description, ITestOutputHelper testOutputHelper)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description), "Please explain your intent by documenting your scenario.");
        }

        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _tasks.Add((Command: "Scenario", $"SCENARIO for {description}", () => Task.CompletedTask));

        _testOutputHelper = testOutputHelper;
    }

    public static ScenarioRunner Create(string description, ITestOutputHelper testOutputHelper)
        => new(description, testOutputHelper);

    public IScenarioRunner Given(string message, Action action)
    {
        AddTask(nameof(Given), message, action);
        return this;
    }

    public IScenarioRunner Given(string message, Func<Task> action)
    {
        AddTask(nameof(Given), message, action);

        return this;
    }

    public IScenarioRunner Given(Action<IStepRunner> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _stepCommand = nameof(Given);

        action.Invoke(this);

        return this;
    }

    public IScenarioRunner When(string message, Action action)
    {
        AddTask(nameof(When), message, action);

        return this;
    }

    public IScenarioRunner When(string message, Func<Task> action)
    {
        AddTask(nameof(When), message, action);

        return this;
    }

    public IScenarioRunner When(Action<IStepRunner> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _stepCommand = nameof(When);

        action.Invoke(this);

        return this;
    }

    public IScenarioRunner Then(string message, Action action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using AssertionScope scope = new();
            return Task.Run(action);
        });

        return this;
    }

    public IScenarioRunner Then(string message, Func<Task> action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using AssertionScope scope = new();
            return action();
        });

        return this;
    }

    public IScenarioRunner Then(Action<IStepRunner> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _stepCommand = nameof(Then);

        action.Invoke(this);

        return this;
    }

    public IScenarioRunner And(string message, Func<Task> action)
    {
        var previousCommand = _tasks.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => Given(message, action)
        };
    }

    public IScenarioRunner And(string message, Action action)
    {
        var previousCommand = _tasks.LastOrDefault();

        if (string.IsNullOrWhiteSpace(previousCommand.Command))
        {
            return Given(message, action);
        }

        return previousCommand.Command switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => Given(message, action)
        };
    }

    public IScenarioRunner And(Action<IStepRunner> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var previousCommand = _tasks.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(action),
            nameof(When) => When(action),
            nameof(Then) => Then(action),
            _ => Given(action)
        };
    }

    public async Task PlayAsync()
    {
        foreach (var task in _tasks)
        {
            _testOutputHelper.WriteLine(task.Description);
            await task.Task();
        }
    }

    public void SetContextData(string contextKey, object data)
    {
        if (_context.ContainsKey(contextKey))
        {
            _context.Remove(contextKey);
        }

        _context.Add(contextKey, data);
    }

    public T GetContextData<T>(string contextKey)
    {
        _context.Should().ContainKey(contextKey);
        return (T)_context[contextKey];
    }

    private void AddTask(string command, string message, Action action) => AddTask(command, message, () => Task.Run(action));

    private void AddTask(string command, string message, Func<Task> action)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException(nameof(message), "Please explain your intent by documenting your test.");
        }

        var previousCommand = _tasks.LastOrDefault();
        string textCommand = command.Equals(previousCommand.Command, StringComparison.OrdinalIgnoreCase) ? "and" : command.ToUpperInvariant();
        string text = $"{textCommand} {message}";
        _tasks.Add((command, $"{text}", () => Task.Run(action)));
    }

    public IStepRunner Execute(string message, Action action)
    {
        _ = _stepCommand switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => Given(message, action)
        };

        return this;
    }

    public IStepRunner Execute(string message, Func<Task> action)
    {
        _ = _stepCommand switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => Given(message, action)
        };

        return this;
    }
}
