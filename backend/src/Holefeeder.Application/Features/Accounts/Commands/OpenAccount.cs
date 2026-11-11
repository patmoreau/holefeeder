using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Features.Accounts.Commands;

public static class OpenAccount
{
    public record Request(AccountType Type, string Name, DateOnly OpenDate, Money OpenBalance, string Description);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
            RuleFor(command => command.OpenDate).NotEmpty();
        }
    }
}
