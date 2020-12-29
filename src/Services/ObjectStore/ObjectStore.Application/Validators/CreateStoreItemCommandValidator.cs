using DrifterApps.Holefeeder.ObjectStore.Application.Commands;

using FluentValidation;

using Microsoft.Extensions.Logging;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Validators
{
    public class CreateStoreItemCommandValidator : AbstractValidator<CreateStoreItemCommand>
    {
        public CreateStoreItemCommandValidator(ILogger<CreateStoreItemCommandValidator> logger)
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Data).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
