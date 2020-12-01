using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models
{
    public class CommandResult<TResult>
    {
        private readonly List<string> _messages;

        public TResult Result { get; }

        public CommandStatus Status { get; }
        
        public IReadOnlyCollection<string> Messages => _messages;
        
        public CommandResult(CommandStatus status, TResult result)
        {
            Status = status;
            Result = result;
            _messages = new List<string>();
        }

        public CommandResult(CommandStatus status, TResult result, string message) : this(status, result)
        {
            _messages.Add(message);
        }

        public CommandResult(CommandStatus status, TResult result, params string[] messages) : this(status, result)
        {
            _messages.AddRange(messages);
        }
    }
}
