using System.ComponentModel.DataAnnotations;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Features.Accounts.Commands;

public static class ModifyAccount
{
    internal record Request([Required] AccountId Id, [MinLength(1), MaxLength(255)] string Name, Money OpenBalance, string Description);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
        }
    }
}
