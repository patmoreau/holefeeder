namespace Holefeeder.FunctionalTests.Features;

public class ScenarioPlayer
{
    private readonly List<(string Command, Func<Task> Task)> _tasks = new();
    private readonly ITestOutputHelper _testOutputHelper;

    private ScenarioPlayer(string description, ITestOutputHelper testOutputHelper)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description), "Please explain your intent by documenting your scenario.");
        }

        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _tasks.Add((Command: "Scenario", () => Task.Run(() => _testOutputHelper!.WriteLine($"SCENARIO for {description}"))));

        _testOutputHelper = testOutputHelper;
    }

    public static ScenarioPlayer Create(string description, ITestOutputHelper testOutputHelper)
        => new(description, testOutputHelper);

    public ScenarioPlayer Given(string message, Action action)
    {
        AddTask(nameof(Given), message, action);
        return this;
    }

    public ScenarioPlayer Given(string message, Func<Task> action)
    {
        AddTask(nameof(Given), message, action);

        return this;
    }

    public ScenarioPlayer When(string message, Action action)
    {
        AddTask(nameof(When), message, action);

        return this;
    }

    public ScenarioPlayer When(string message, Func<Task> action)
    {
        AddTask(nameof(When), message, action);

        return this;
    }

    public ScenarioPlayer Then(string message, Action action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using AssertionScope scope = new AssertionScope();
            return Task.Run(action);
        });

        return this;
    }

    public ScenarioPlayer Then(string message, Func<Task> action)
    {
        AddTask(nameof(Then), message, () =>
        {
            using AssertionScope scope = new AssertionScope();
            return action();
        });

        return this;
    }

    public ScenarioPlayer And(string message, Func<Task> action)
    {
        (string Command, Func<Task> Task) previousCommand = _tasks.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => throw new InvalidOperationException($"A command needs to be done first. Either {nameof(Given)}, {nameof(When)} or {nameof(Then)}.")
        };
    }

    public ScenarioPlayer And(string message, Action action)
    {
        (string Command, Func<Task> Task) previousCommand = _tasks.LastOrDefault();

        return previousCommand.Command switch
        {
            nameof(Given) => Given(message, action),
            nameof(When) => When(message, action),
            nameof(Then) => Then(message, action),
            _ => throw new InvalidOperationException($"A command needs to be done first. Either {nameof(Given)}, {nameof(When)} or {nameof(Then)}.")
        };
    }

    public async Task PlayAsync()
    {
        foreach ((string Command, Func<Task> Task) task in _tasks)
        {
            await task.Task();
        }
    }

    private void AddTask(string command, string message, Action action) => AddTask(command, message, () => Task.Run(action));

    private void AddTask(string command, string message, Func<Task> action)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentNullException(nameof(message), "Please explain your intent by documenting your test.");
        }

        (string Command, Func<Task> Task) previousCommand = _tasks.LastOrDefault();
        string textCommand = command.Equals(previousCommand.Command, StringComparison.OrdinalIgnoreCase) ? "and" : command.ToUpperInvariant();
        string text = $"{textCommand} {message}";
        _tasks.Add((command, () => Task.Run(() => _testOutputHelper.WriteLine($"{text}"))));
        _tasks.Add((command, () => Task.Run(action)));
    }
}
