using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

using FluentValidation;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public class CommandResult<TResult>
    {
        public TResult Result { get; }

        public CommandStatus Status { get; }

        public ImmutableArray<string> Messages { get; }

        public CommandResult(CommandStatus status, TResult result) => 
            (Status, Result, Messages) = (status, result, ImmutableArray<string>.Empty);
        
        public CommandResult(CommandStatus status, TResult result, params string[] messages) : this(status, result)
        {
            Messages = messages.ToImmutableArray();
        }
        
        public CommandResult(CommandStatus status, TResult result, ValidationException exception) : this(status, result)
        {
            Messages = exception.Errors.Select(x => x.ToString()).ToImmutableArray();
        }

        [JsonConstructor]
        public CommandResult(CommandStatus status, TResult result, ImmutableArray<string> messages) =>
            (Status, Result, Messages) = (status, result, messages);
    }
}
