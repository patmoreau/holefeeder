using System.Collections.Generic;
using System.Collections.Immutable;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public record CommandResult(CommandStatus Status, ImmutableArray<string> Messages)
{
    public static CommandResult Create(CommandStatus status)
    {
        return new(status, ImmutableArray<string>.Empty);
    }

    public static CommandResult Create(CommandStatus status, string message)
    {
        return new(status, ImmutableArray.Create(message));
    }

    public static CommandResult Create(CommandStatus status, IEnumerable<string> messages)
    {
        return new(status, messages.ToImmutableArray());
    }
}

public record CommandResult<TResult>
    (CommandStatus Status, TResult Result, ImmutableArray<string> Messages) : CommandResult(Status, Messages)
{
    public static CommandResult<T> Create<T>(CommandStatus status, T result)
    {
        return new(status, result, ImmutableArray<string>.Empty);
    }

    public static CommandResult<T> Create<T>(CommandStatus status, T result, string message)
    {
        return new(status, result, ImmutableArray.Create(message));
    }

    public static CommandResult<T> Create<T>(CommandStatus status, T result, IEnumerable<string> messages)
    {
        return new(status, result, messages.ToImmutableArray());
    }
}
