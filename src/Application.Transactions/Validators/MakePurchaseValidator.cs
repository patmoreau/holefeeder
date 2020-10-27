using DrifterApps.Holefeeder.Application.Transactions.Commands;
using FluentValidation;

namespace DrifterApps.Holefeeder.Application.Transactions.Validators
{
    public class MakePurchaseValidator : AbstractValidator<MakePurchaseCommand>
    {
        public MakePurchaseValidator()
        {
            RuleFor(command => command.AccountId).NotNull().NotEmpty();
            RuleFor(command => command.CategoryId).NotNull().NotEmpty();
            RuleFor(command => command.Date).NotNull();
        }
    }
}
