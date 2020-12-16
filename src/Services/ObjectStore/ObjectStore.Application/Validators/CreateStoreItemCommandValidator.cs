using DrifterApps.Holefeeder.ObjectStore.Application.Commands;
using FluentValidation;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Validators
{
    public class CreateStoreItemCommandValidator : AbstractValidator<CreateStoreItemCommand>
    {
        public CreateStoreItemCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty();
            RuleFor(x => x.Data)
                .NotEmpty();
        }
    }}
