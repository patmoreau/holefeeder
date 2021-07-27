using DrifterApps.Holefeeder.Budgeting.Application.Accounts.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Validators
{
    public class FavoriteAccountValidator : AbstractValidator<FavoriteAccountCommand>
    {
        public FavoriteAccountValidator(ILogger<FavoriteAccountValidator> logger)
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
